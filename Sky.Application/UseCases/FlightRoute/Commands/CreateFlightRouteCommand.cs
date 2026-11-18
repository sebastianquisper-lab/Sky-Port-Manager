using MediatR;
using Sky.Domain.Ports;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;

namespace Sky.Application.UseCases.FlightRoute.Commands;

public sealed record CreateFlightRouteCommand(
    int OriginAirportId,
    int DestinationAirportId,
    decimal DistanceKm,
    int EstimatedDurationMinutes) : IRequest<FlightRouteEntity>;

internal sealed class CreateFlightRouteHandler(IUnitOfWork uow)
    : IRequestHandler<CreateFlightRouteCommand, FlightRouteEntity>
{
    public async Task<FlightRouteEntity> Handle(CreateFlightRouteCommand request, CancellationToken ct)
    {
        var entity = new FlightRouteEntity
        {
            OriginAirportId = request.OriginAirportId,
            DestinationAirportId = request.DestinationAirportId,
            DistanceKm = request.DistanceKm,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
            IsActive = true
        };

        await uow.Repository<FlightRouteEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
