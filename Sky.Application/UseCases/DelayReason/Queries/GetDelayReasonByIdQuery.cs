using MediatR;
using Sky.Domain.Ports;
using DelayReasonEntity = Sky.Domain.Entities.DelayReason;

namespace Sky.Application.UseCases.DelayReason.Queries;

public sealed record GetDelayReasonByIdQuery(
    int DelayReasonId) : IRequest<DelayReasonEntity?>;

internal sealed class GetDelayReasonByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetDelayReasonByIdQuery, DelayReasonEntity?>
{
    public Task<DelayReasonEntity?> Handle(GetDelayReasonByIdQuery request, CancellationToken ct)
        => uow.Repository<DelayReasonEntity>().GetByIdAsync(request.DelayReasonId, ct);
}
