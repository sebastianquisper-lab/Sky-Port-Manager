using MediatR;
using Sky.Domain.Ports;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;

namespace Sky.Application.UseCases.PaymentMethod.Queries;

public sealed record GetPaymentMethodsQuery : IRequest<IReadOnlyList<PaymentMethodEntity>>;

internal sealed class GetPaymentMethodsHandler(IUnitOfWork uow)
    : IRequestHandler<GetPaymentMethodsQuery, IReadOnlyList<PaymentMethodEntity>>
{
    public Task<IReadOnlyList<PaymentMethodEntity>> Handle(GetPaymentMethodsQuery request, CancellationToken ct)
        => uow.Repository<PaymentMethodEntity>().GetAllAsync(ct);
}
