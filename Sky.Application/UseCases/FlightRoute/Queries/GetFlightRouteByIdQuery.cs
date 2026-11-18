using MediatR;
using Sky.Domain.Ports;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;

namespace Sky.Application.UseCases.FlightRoute.Queries;

public sealed record GetFlightRouteByIdQuery(
    int RouteId) : IRequest<FlightRouteEntity?>;

internal sealed class GetFlightRouteByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightRouteByIdQuery, FlightRouteEntity?>
{
    public Task<FlightRouteEntity?> Handle(GetFlightRouteByIdQuery request, CancellationToken ct)
        => uow.Repository<FlightRouteEntity>().GetByIdAsync(request.RouteId, ct);
}
