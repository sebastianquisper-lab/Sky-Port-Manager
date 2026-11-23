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
using BoardingGateEntity = Sky.Domain.Entities.BoardingGate;
using BoardingPassEntity = Sky.Domain.Entities.BoardingPass;

namespace Sky.Application.UseCases.BoardingPass.Queries;

public sealed record GetBoardingPassPdfQuery(int BoardingPassId) : IRequest<FileDto>;

internal sealed class GetBoardingPassPdfHandler(IUnitOfWork uow, IDocumentGenerator documents)
    : IRequestHandler<GetBoardingPassPdfQuery, FileDto>
{
    public async Task<FileDto> Handle(GetBoardingPassPdfQuery request, CancellationToken ct)
    {
        var pass = await uow.Repository<BoardingPassEntity>().GetByIdAsync(request.BoardingPassId, ct)
            ?? throw new KeyNotFoundException($"El pase de abordar {request.BoardingPassId} no existe.");

        var ticket = await uow.Repository<TicketEntity>().GetByIdAsync(pass.TicketId, ct)
            ?? throw new KeyNotFoundException($"El boleto {pass.TicketId} no existe.");
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

        // El vuelo puede no tener puerta asignada todavía.
        BoardingGateEntity? gate = flight.DepartureGateId is int gateId
            ? await uow.Repository<BoardingGateEntity>().GetByIdAsync(gateId, ct)
            : null;

        var pdf = await documents.GenerateBoardingPassPdfAsync(
            pass, ticket, passenger, cabinClass, flight, airline, origin, destination, gate, ct);

        return new FileDto($"boarding-pass-{ticket.TicketNumber}.pdf", "application/pdf", pdf);
    }
}
