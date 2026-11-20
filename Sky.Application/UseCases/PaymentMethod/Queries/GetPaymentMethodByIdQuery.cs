using MediatR;
using Sky.Domain.Ports;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;

namespace Sky.Application.UseCases.PaymentMethod.Queries;

public sealed record GetPaymentMethodByIdQuery(
    int PaymentMethodId) : IRequest<PaymentMethodEntity?>;

internal sealed class GetPaymentMethodByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethodEntity?>
{
    public Task<PaymentMethodEntity?> Handle(GetPaymentMethodByIdQuery request, CancellationToken ct)
        => uow.Repository<PaymentMethodEntity>().GetByIdAsync(request.PaymentMethodId, ct);
}
