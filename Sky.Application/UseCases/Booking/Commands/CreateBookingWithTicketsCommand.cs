using MediatR;
using Sky.Application.DTOs;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;
using BookingEntity = Sky.Domain.Entities.Booking;
using BookingFlightPassengerEntity = Sky.Domain.Entities.BookingFlightPassenger;
using BookingStatusEntity = Sky.Domain.Entities.BookingStatus;
using FlightEntity = Sky.Domain.Entities.Flight;
using PaymentEntity = Sky.Domain.Entities.Payment;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;
using TicketEntity = Sky.Domain.Entities.Ticket;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.Booking.Commands;

public sealed record BookingItem(
    int FlightId,
    int PassengerId,
    int CabinClassId,
    int? FareId,
    string? SeatNumber,
    decimal Price,
    decimal TaxAmount);

/// <summary>
/// Reserva completa en una sola transacción: reserva + pasajeros por vuelo + boletos + cobro + pago.
/// Si algo falla (cupo lleno, pago rechazado...) no queda nada guardado.
/// </summary>
public sealed record CreateBookingWithTicketsCommand(
    string? ContactEmail,
    string? ContactPhone,
    string CurrencyCode,
    int PaymentMethodId,
    string? PaymentToken,
    IReadOnlyList<BookingItem> Items) : IRequest<BookingConfirmationDto>;

internal sealed class CreateBookingWithTicketsHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICodeGenerator codes,
    IPaymentGateway gateway)
    : IRequestHandler<CreateBookingWithTicketsCommand, BookingConfirmationDto>
{
    // Nombres que deben existir en las tablas de estados (booking_status, ticket_status, payment_status).
    private const string ConfirmedBooking = "CONFIRMED";
    private const string IssuedTicket = "ISSUED";
    private const string CompletedPayment = "COMPLETED";

    public Task<BookingConfirmationDto> Handle(CreateBookingWithTicketsCommand request, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("La reserva debe tener al menos un boleto.");
        }

        return uow.ExecuteInTransactionAsync(innerCt => CreateAsync(request, innerCt), ct);
    }

    private async Task<BookingConfirmationDto> CreateAsync(CreateBookingWithTicketsCommand request, CancellationToken ct)
    {
        var method = await uow.Repository<PaymentMethodEntity>().GetByIdAsync(request.PaymentMethodId, ct)
            ?? throw new KeyNotFoundException($"El método de pago {request.PaymentMethodId} no existe.");

        var bookingStatus = await uow.Repository<BookingStatusEntity>()
            .FirstOrDefaultAsync(s => s.StatusName == ConfirmedBooking, ct)
            ?? throw new KeyNotFoundException($"El estado de reserva '{ConfirmedBooking}' no existe.");

        var ticketStatus = await uow.Repository<TicketStatusEntity>()
            .FirstOrDefaultAsync(s => s.StatusName == IssuedTicket, ct)
            ?? throw new KeyNotFoundException($"El estado de boleto '{IssuedTicket}' no existe.");

        var paymentStatus = await uow.Repository<PaymentStatusEntity>()
            .FirstOrDefaultAsync(s => s.StatusName == CompletedPayment, ct)
            ?? throw new KeyNotFoundException($"El estado de pago '{CompletedPayment}' no existe.");

        var now = clock.UtcNow;

        var booking = new BookingEntity
        {
            BookingStatusId = bookingStatus.BookingStatusId,
            BookingCode = codes.GenerateBookingCode(),
            BookingDateUtc = now,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };
        await uow.Repository<BookingEntity>().AddAsync(booking, ct);
        await uow.SaveChangesAsync(ct); // genera booking.BookingId

        // Cada pasajero debe estar registrado en la reserva para el vuelo antes de emitir su boleto.
        foreach (var pair in request.Items.Select(i => (i.FlightId, i.PassengerId)).Distinct())
        {
            await uow.Repository<BookingFlightPassengerEntity>().AddAsync(new BookingFlightPassengerEntity
            {
                BookingId = booking.BookingId,
                FlightId = pair.FlightId,
                PassengerId = pair.PassengerId
            }, ct);
        }
        await uow.SaveChangesAsync(ct);

        var tickets = new List<TicketEntity>();
        foreach (var item in request.Items)
        {
            var flight = await uow.Repository<FlightEntity>().GetByIdAsync(item.FlightId, ct)
                ?? throw new KeyNotFoundException($"El vuelo {item.FlightId} no existe.");

            var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(flight.AirlineId, ct)
                ?? throw new KeyNotFoundException($"La aerolínea {flight.AirlineId} no existe.");

            var ticket = new TicketEntity
            {
                BookingId = booking.BookingId,
                FlightId = item.FlightId,
                PassengerId = item.PassengerId,
                TicketStatusId = ticketStatus.TicketStatusId,
                CabinClassId = item.CabinClassId,
                FareId = item.FareId,
                TicketNumber = codes.GenerateTicketNumber(airline.IataCode),
                SeatNumber = item.SeatNumber,
                IssueDateUtc = now,
                Price = item.Price,
                TaxAmount = item.TaxAmount,
                CurrencyCode = request.CurrencyCode
            };

            await uow.Repository<TicketEntity>().AddAsync(ticket, ct);
            tickets.Add(ticket);
        }
        await uow.SaveChangesAsync(ct); // aquí el trigger de capacidad puede rechazar un vuelo lleno

        var total = request.Items.Sum(i => i.Price + i.TaxAmount);

        var transactionReference = await gateway.ChargeAsync(
            total, request.CurrencyCode, $"Reserva {booking.BookingCode}",
            request.ContactEmail, method.MethodName, request.PaymentToken, ct);

        if (string.IsNullOrWhiteSpace(transactionReference))
        {
            throw new InvalidOperationException("La pasarela rechazó el pago; la reserva no se creó.");
        }

        var payment = new PaymentEntity
        {
            BookingId = booking.BookingId,
            PaymentMethodId = request.PaymentMethodId,
            PaymentStatusId = paymentStatus.PaymentStatusId,
            CurrencyCode = request.CurrencyCode,
            Amount = total,
            TransactionReference = transactionReference,
            CreatedAtUtc = now,
            PaidAtUtc = now
        };

        try
        {
            await uow.Repository<PaymentEntity>().AddAsync(payment, ct);
            await uow.SaveChangesAsync(ct);
        }
        catch
        {
            // Compensación: ya se cobró pero no se pudo guardar, así que se devuelve el dinero.
            await gateway.RefundAsync(transactionReference, total, request.CurrencyCode, CancellationToken.None);
            throw;
        }

        return new BookingConfirmationDto(booking, tickets, payment);
    }
}
