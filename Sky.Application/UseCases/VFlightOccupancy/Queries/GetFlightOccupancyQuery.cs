using MediatR;
using Sky.Domain.Ports;
using VFlightOccupancyEntity = Sky.Domain.Entities.VFlightOccupancy;

namespace Sky.Application.UseCases.VFlightOccupancy.Queries;

public sealed record GetFlightOccupancyQuery : IRequest<IReadOnlyList<VFlightOccupancyEntity>>;

internal sealed class GetFlightOccupancyHandler(IUnitOfWork uow)
    : IRequestHandler<GetFlightOccupancyQuery, IReadOnlyList<VFlightOccupancyEntity>>
{
    public Task<IReadOnlyList<VFlightOccupancyEntity>> Handle(GetFlightOccupancyQuery request, CancellationToken ct)
        => uow.Repository<VFlightOccupancyEntity>().GetAllAsync(ct);
}
