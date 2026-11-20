using MediatR;
using Sky.Domain.Ports;
using PaymentEntity = Sky.Domain.Entities.Payment;

namespace Sky.Application.UseCases.Payment.Queries;

public sealed record GetPaymentsQuery : IRequest<IReadOnlyList<PaymentEntity>>;

internal sealed class GetPaymentsHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentsQuery, IReadOnlyList<PaymentEntity>>
{
    public Task<IReadOnlyList<PaymentEntity>> Handle(GetPaymentsQuery request, CancellationToken ct)
        => uow.Repository<PaymentEntity>().GetAllAsync(ct);
}
