using MediatR;
using Sky.Domain.Ports;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.PaymentStatus.Queries;

public sealed record GetPaymentStatusesQuery : IRequest<IReadOnlyList<PaymentStatusEntity>>;

internal sealed class GetPaymentStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentStatusesQuery, IReadOnlyList<PaymentStatusEntity>>
{
    public Task<IReadOnlyList<PaymentStatusEntity>> Handle(GetPaymentStatusesQuery request, CancellationToken ct)
        => uow.Repository<PaymentStatusEntity>().GetAllAsync(ct);
}
