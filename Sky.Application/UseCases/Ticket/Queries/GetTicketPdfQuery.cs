using MediatR;
using Sky.Application.DTOs;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using AirlineEntity = Sky.Domain.Entities.Airline;
using AirportEntity = Sky.Domain.Entities.Airport;
using BookingEntity = Sky.Domain.Entities.Booking;
using CabinClassEntity = Sky.Domain.Entities.CabinClass;
using FlightEntity = Sky.Domain.Entities.Flight;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;
using PassengerEntity = Sky.Domain.Entities.Passenger;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Queries;

public sealed record GetTicketPdfQuery(int TicketId) : IRequest<FileDto>;

internal sealed class GetTicketPdfHandler(IUnitOfWork uow, IDocumentGenerator documents)
    : IRequestHandler<GetTicketPdfQuery, FileDto>
{
    public async Task<FileDto> Handle(GetTicketPdfQuery request, CancellationToken ct)
    {
        var ticket = await uow.Repository<TicketEntity>().GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException($"El boleto {request.TicketId} no existe.");

        var passenger = await uow.Repository<PassengerEntity>().GetByIdAsync(ticket.PassengerId, ct)
            ?? throw new KeyNotFoundException($"El pasajero {ticket.PassengerId} no existe.");

        var booking = await uow.Repository<BookingEntity>().GetByIdAsync(ticket.BookingId, ct)
            ?? throw new KeyNotFoundException($"La reserva {ticket.BookingId} no existe.");

        var cabinClass = await uow.Repository<CabinClassEntity>().GetByIdAsync(ticket.CabinClassId, ct)
            ?? throw new KeyNotFoundException($"La clase de cabina {ticket.CabinClassId} no existe.");

        var flight = await uow.Repository<FlightEntity>().GetByIdAsync(ticket.FlightId, ct)
            ?? throw new KeyNotFoundException($"El vuelo {ticket.FlightId} no existe.");

        var airline = await uow.Repository<AirlineEntity>().GetByIdAsync(flight.AirlineId, ct)
            ?? throw new KeyNotFoundException($"La aerolínea {flight.AirlineId} no existe.");

        var route = await uow.Repository<FlightRouteEntity>().GetByIdAsync(flight.RouteId, ct)
            ?? throw new KeyNotFoundException($"La ruta {flight.RouteId} no existe.");

        var origin = await uow.Repository<AirportEntity>().GetByIdAsync(route.OriginAirportId, ct)
            ?? throw new KeyNotFoundException($"El aeropuerto {route.OriginAirportId} no existe.");

        var destination = await uow.Repository<AirportEntity>().GetByIdAsync(route.DestinationAirportId, ct)
            ?? throw new KeyNotFoundException($"El aeropuerto {route.DestinationAirportId} no existe.");

        var pdf = await documents.GenerateTicketPdfAsync(
            ticket, passenger, booking, cabinClass, flight, airline, origin, destination, ct);

        return new FileDto($"ticket-{ticket.TicketNumber}.pdf", "application/pdf", pdf);
    }
}
