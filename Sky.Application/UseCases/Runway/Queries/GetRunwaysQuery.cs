using MediatR;
using Sky.Domain.Ports;
using RunwayEntity = Sky.Domain.Entities.Runway;

namespace Sky.Application.UseCases.Runway.Queries;

public sealed record GetRunwaysQuery : IRequest<IReadOnlyList<RunwayEntity>>;

internal sealed class GetRunwaysHandler(IUnitOfWork uow)
    : IRequestHandler<GetRunwaysQuery, IReadOnlyList<RunwayEntity>>
{
    public Task<IReadOnlyList<RunwayEntity>> Handle(GetRunwaysQuery request, CancellationToken ct)
        => uow.Repository<RunwayEntity>().GetAllAsync(ct);
}
