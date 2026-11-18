using MediatR;
using Sky.Domain.Ports;
using FlightEntity = Sky.Domain.Entities.Flight;

namespace Sky.Application.UseCases.Flight.Commands;

public sealed record CreateFlightCommand(
    int AirlineId,
    int AircraftId,
    int RouteId,
    int FlightStatusId,
    string FlightNumber,
    DateTime ScheduledDepartureUtc,
    DateTime ScheduledArrivalUtc,
    DateTime? ActualDepartureUtc,
    DateTime? ActualArrivalUtc,
    int? DepartureGateId,
    int? ArrivalGateId) : IRequest<FlightEntity>;

internal sealed class CreateFlightHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFlightCommand, FlightEntity>
{
    public async Task<FlightEntity> Handle(CreateFlightCommand request, CancellationToken ct)
    {
        var entity = new FlightEntity
        {
            AirlineId = request.AirlineId,
            AircraftId = request.AircraftId,
            RouteId = request.RouteId,
            FlightStatusId = request.FlightStatusId,
            FlightNumber = request.FlightNumber,
            ScheduledDepartureUtc = request.ScheduledDepartureUtc,
            ScheduledArrivalUtc = request.ScheduledArrivalUtc,
            ActualDepartureUtc = request.ActualDepartureUtc,
            ActualArrivalUtc = request.ActualArrivalUtc,
            DepartureGateId = request.DepartureGateId,
            ArrivalGateId = request.ArrivalGateId
        };

        await uow.Repository<FlightEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
