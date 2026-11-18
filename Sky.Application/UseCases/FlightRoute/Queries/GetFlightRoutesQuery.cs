using MediatR;
using Sky.Domain.Ports;
using FlightRouteEntity = Sky.Domain.Entities.FlightRoute;

namespace Sky.Application.UseCases.FlightRoute.Queries;

public sealed record GetFlightRoutesQuery : IRequest<IReadOnlyList<FlightRouteEntity>>;

internal sealed class GetFlightRoutesHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightRoutesQuery, IReadOnlyList<FlightRouteEntity>>
{
    public Task<IReadOnlyList<FlightRouteEntity>> Handle(GetFlightRoutesQuery request, CancellationToken ct)
        => uow.Repository<FlightRouteEntity>().GetAllAsync(ct);
}
