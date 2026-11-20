using MediatR;
using Sky.Domain.Ports;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.PaymentStatus.Queries;

public sealed record GetPaymentStatusByIdQuery(
    int PaymentStatusId) : IRequest<PaymentStatusEntity?>;

internal sealed class GetPaymentStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentStatusByIdQuery, PaymentStatusEntity?>
{
    public Task<PaymentStatusEntity?> Handle(GetPaymentStatusByIdQuery request, CancellationToken ct)
        => uow.Repository<PaymentStatusEntity>().GetByIdAsync(request.PaymentStatusId, ct);
}
