using MediatR;
using Sky.Domain.Ports;
using RefundEntity = Sky.Domain.Entities.Refund;

namespace Sky.Application.UseCases.Refund.Queries;

public sealed record GetRefundByIdQuery(
    int RefundId) : IRequest<RefundEntity?>;

internal sealed class GetRefundByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetRefundByIdQuery, RefundEntity?>
{
    public Task<RefundEntity?> Handle(GetRefundByIdQuery request, CancellationToken ct)
        => uow.Repository<RefundEntity>().GetByIdAsync(request.RefundId, ct);
}
