using MediatR;
using Sky.Domain.Ports;
using FlightEntity = Sky.Domain.Entities.Flight;

namespace Sky.Application.UseCases.Flight.Commands;

public sealed record UpdateFlightCommand(
    int FlightId,
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
    int? ArrivalGateId) : IRequest<FlightEntity?>;

internal sealed class UpdateFlightHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightCommand, FlightEntity?>
{
    public async Task<FlightEntity?> Handle(UpdateFlightCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightEntity>().GetByIdAsync(request.FlightId, ct);
        if (entity is null) return null;

        entity.AirlineId = request.AirlineId;
        entity.AircraftId = request.AircraftId;
        entity.RouteId = request.RouteId;
        entity.FlightStatusId = request.FlightStatusId;
        entity.FlightNumber = request.FlightNumber;
        entity.ScheduledDepartureUtc = request.ScheduledDepartureUtc;
        entity.ScheduledArrivalUtc = request.ScheduledArrivalUtc;
        entity.ActualDepartureUtc = request.ActualDepartureUtc;
        entity.ActualArrivalUtc = request.ActualArrivalUtc;
        entity.DepartureGateId = request.DepartureGateId;
        entity.ArrivalGateId = request.ArrivalGateId;

        uow.Repository<FlightEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
