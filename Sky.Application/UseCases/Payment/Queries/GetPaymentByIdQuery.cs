using MediatR;
using Sky.Domain.Ports;
using PaymentEntity = Sky.Domain.Entities.Payment;

namespace Sky.Application.UseCases.Payment.Queries;

public sealed record GetPaymentByIdQuery(
    int PaymentId) : IRequest<PaymentEntity?>;

internal sealed class GetPaymentByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentByIdQuery, PaymentEntity?>
{
    public Task<PaymentEntity?> Handle(GetPaymentByIdQuery request, CancellationToken ct)
        => uow.Repository<PaymentEntity>().GetByIdAsync(request.PaymentId, ct);
}
