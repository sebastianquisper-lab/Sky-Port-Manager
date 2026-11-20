using MediatR;
using Sky.Domain.Ports;
using RunwayEntity = Sky.Domain.Entities.Runway;

namespace Sky.Application.UseCases.Runway.Queries;

public sealed record GetRunwayByIdQuery(
    int RunwayId) : IRequest<RunwayEntity?>;

internal sealed class GetRunwayByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetRunwayByIdQuery, RunwayEntity?>
{
    public Task<RunwayEntity?> Handle(GetRunwayByIdQuery request, CancellationToken ct)
        => uow.Repository<RunwayEntity>().GetByIdAsync(request.RunwayId, ct);
}
