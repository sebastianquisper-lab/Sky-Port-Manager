using MediatR;
using Sky.Domain.Ports;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;

namespace Sky.Application.UseCases.DelayReason.Queries;

public sealed record GetDelayReasonsQuery : IRequest<IReadOnlyList<DelayReasonEntity>>;

internal sealed class GetDelayReasonsHandler(IUnitOfWork uow)
    : IRequestHandler<GetDelayReasonsQuery, IReadOnlyList<DelayReasonEntity>>
{
    public Task<IReadOnlyList<DelayReasonEntity>> Handle(GetDelayReasonsQuery request, CancellationToken ct)
        => uow.Repository<DelayReasonEntity>().GetAllAsync(ct);
}
