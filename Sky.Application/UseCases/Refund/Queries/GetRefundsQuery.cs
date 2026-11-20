using MediatR;
using Sky.Domain.Ports;
using RefundEntity = Sky.Domain.Entities.Refund;

namespace Sky.Application.UseCases.Refund.Queries;

public sealed record GetRefundsQuery : IRequest<IReadOnlyList<RefundEntity>>;

internal sealed class GetRefundsHandler(IUnitOfWork uow)
    : IRequestHandler<GetRefundsQuery, IReadOnlyList<RefundEntity>>
{
    public Task<IReadOnlyList<RefundEntity>> Handle(GetRefundsQuery request, CancellationToken ct)
        => uow.Repository<RefundEntity>().GetAllAsync(ct);
}
