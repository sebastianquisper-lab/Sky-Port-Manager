using MediatR;
using Sky.Domain.Ports;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;

namespace Sky.Application.UseCases.PaymentMethod.Commands;

public sealed record CreatePaymentMethodCommand(
    string MethodName) : IRequest<PaymentMethodEntity>;

internal sealed class CreatePaymentMethodHandler(IUnitOfWork uow)
    : IRequestHandler<CreatePaymentMethodCommand, PaymentMethodEntity>
{
    public async Task<PaymentMethodEntity> Handle(CreatePaymentMethodCommand request, CancellationToken ct)
    {
        var entity = new PaymentMethodEntity
        {
            MethodName = request.MethodName
        };

        await uow.Repository<PaymentMethodEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
