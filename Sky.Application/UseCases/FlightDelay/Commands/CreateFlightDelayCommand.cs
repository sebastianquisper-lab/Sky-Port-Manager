using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;
using AirportEntity = Sky.Domain.Entities.Airport;
using BookingEntity = Sky.Domain.Entities.Booking;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;
using FlightDelayEntity = Sky.Domain.Entities.FlightDelay;
using FlightEntity = Sky.Domain.Entities.Flight;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.FlightDelay.Commands;

public sealed record CreateFlightDelayCommand(
    int FlightId,
    int DelayReasonId,
    int DelayMinutes,
    string? Notes) : IRequest<FlightDelayEntity>;

internal sealed class CreateFlightDelayHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICurrentUserService currentUser,
    IEmailSender emailSender,
    ILogger<CreateFlightDelayHandler> logger)
    : IRequestHandler<CreateFlightDelayCommand, FlightDelayEntity>
{
    public async Task<FlightDelayEntity> Handle(CreateFlightDelayCommand request, CancellationToken ct)
    {
        var flight = await uow.Repository<FlightEntity>().GetByIdAsync(request.FlightId, ct)
            ?? throw new KeyNotFoundException($"El vuelo {request.FlightId} no existe.");

        var reason = await uow.Repository<DelayReasonEntity>().GetByIdAsync(request.DelayReasonId, ct)
            ?? throw new KeyNotFoundException($"El motivo de retraso {request.DelayReasonId} no existe.");

        var entity = new FlightDelayEntity
        {
            FlightId = request.FlightId,
            DelayReasonId = request.DelayReasonId,
            RegisteredByEmployeeId = currentUser.EmployeeId,
            DelayMinutes = request.DelayMinutes,
            Notes = request.Notes,
            RegisteredAtUtc = clock.UtcNow
        };

        await uow.Repository<FlightDelayEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        // El aviso es "mejor esfuerzo": si el correo falla, el retraso ya quedó registrado.
        try
        {
            await NotifyPassengersAsync(flight, reason, request, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo notificar el retraso del vuelo {FlightId}", request.FlightId);
        }

        return entity;
    }

    private async Task NotifyPassengersAsync(
        FlightEntity flight,
        DelayReasonEntity reason,
        CreateFlightDelayCommand request,
        CancellationToken ct)
    {
        var flightId = flight.FlightId;
        var tickets = await uow.Repository<TicketEntity>().FindAsync(t => t.FlightId == flightId, ct);
        var bookingIds = tickets.Select(t => t.BookingId).Distinct().ToList();
        if (bookingIds.Count == 0) return;

        var bookings = await uow.Repository<BookingEntity>().FindAsync(b => bookingIds.Contains(b.BookingId), ct);

        var recipients = bookings
            .Select(b => b.ContactEmail)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (recipients.Count == 0) return;

        var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(flight.AirlineId, ct)
            ?? throw new KeyNotFoundException($"La aerolínea {flight.AirlineId} no existe.");

        var route = await uow.Repository<FlightRouteEntity>().GetByIdAsync(flight.RouteId, ct)
            ?? throw new KeyNotFoundException($"La ruta {flight.RouteId} no existe.");

        var origin = await uow.Repository<AirportEntity>().GetByIdAsync(route.OriginAirportId, ct)
            ?? throw new KeyNotFoundException($"El aeropuerto {route.OriginAirportId} no existe.");

        var destination = await uow.Repository<AirportEntity>().GetByIdAsync(route.DestinationAirportId, ct)
            ?? throw new KeyNotFoundException($"El aeropuerto {route.DestinationAirportId} no existe.");

        var flightCode = $"{airline.IataCode}{flight.FlightNumber}";
        var subject = $"Retraso del vuelo {flightCode} {origin.IataCode} → {destination.IataCode}";
        var body =
            $"<p>Su vuelo <b>{WebUtility.HtmlEncode(flightCode)}</b> " +
            $"({origin.IataCode} → {destination.IataCode}) tiene un retraso de " +
            $"<b>{request.DelayMinutes} minutos</b>.</p>" +
            $"<p>Motivo: {WebUtility.HtmlEncode(reason.Description)}.</p>" +
            (string.IsNullOrWhiteSpace(request.Notes) ? "" : $"<p>{WebUtility.HtmlEncode(request.Notes)}</p>") +
            "<p>Disculpe las molestias.</p>";

        foreach (var recipient in recipients)
        {
            await emailSender.SendAsync(recipient, subject, body, ct: ct);
        }
    }
}
