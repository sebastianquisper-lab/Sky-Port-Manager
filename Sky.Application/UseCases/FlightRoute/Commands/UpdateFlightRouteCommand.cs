using MediatR;
using Sky.Domain.Ports;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;

namespace Sky.Application.UseCases.FlightRoute.Commands;

public sealed record UpdateFlightRouteCommand(
    int RouteId,
    int OriginAirportId,
    int DestinationAirportId,
    decimal DistanceKm,
    int EstimatedDurationMinutes,
    bool IsActive) : IRequest<FlightRouteEntity?>;

internal sealed class UpdateFlightRouteHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateFlightRouteCommand, FlightRouteEntity?>
{
    public async Task<FlightRouteEntity?> Handle(UpdateFlightRouteCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<FlightRouteEntity>().GetByIdAsync(request.RouteId, ct);
        if (entity is null) return null;

        entity.OriginAirportId = request.OriginAirportId;
        entity.DestinationAirportId = request.DestinationAirportId;
        entity.DistanceKm = request.DistanceKm;
        entity.EstimatedDurationMinutes = request.EstimatedDurationMinutes;
        entity.IsActive = request.IsActive;

        uow.Repository<FlightRouteEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
