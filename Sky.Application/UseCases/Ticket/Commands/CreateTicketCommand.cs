using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;
using FlightEntity = Sky.Domain.Entities.Flight;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Commands;

/// <summary>
/// La capacidad del vuelo y la pertenencia del pasajero a la reserva las valida PostgreSQL
/// (trigger trg_ticket_validate_capacity y FK compuesta a booking_flight_passenger).
/// </summary>
public sealed record CreateTicketCommand(
    int BookingId,
    int FlightId,
    int PassengerId,
    int TicketStatusId,
    int CabinClassId,
    int? FareId,
    string? SeatNumber,
    decimal Price,
    decimal TaxAmount,
    string CurrencyCode) : IRequest<TicketEntity>;

internal sealed class CreateTicketHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICodeGenerator codes)
    : IRequestHandler<CreateTicketCommand, TicketEntity>
{
    public async Task<TicketEntity> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        // El número de boleto se genera con el código IATA de la aerolínea del vuelo.
        var flight = await uow.Repository<FlightEntity>().GetByIdAsync(request.FlightId, ct)
            ?? throw new KeyNotFoundException($"El vuelo {request.FlightId} no existe.");

        var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(flight.AirlineId, ct)
            ?? throw new KeyNotFoundException($"La aerolínea {flight.AirlineId} no existe.");

        var entity = new TicketEntity
        {
            BookingId = request.BookingId,
            FlightId = request.FlightId,
            PassengerId = request.PassengerId,
            TicketStatusId = request.TicketStatusId,
            CabinClassId = request.CabinClassId,
            FareId = request.FareId,
            TicketNumber = codes.GenerateTicketNumber(airline.IataCode),
            SeatNumber = request.SeatNumber,
            IssueDateUtc = clock.UtcNow,
            Price = request.Price,
            TaxAmount = request.TaxAmount,
            CurrencyCode = request.CurrencyCode
        };

        await uow.Repository<TicketEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
