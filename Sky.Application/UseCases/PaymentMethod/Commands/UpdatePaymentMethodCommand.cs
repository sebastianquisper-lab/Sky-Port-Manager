using MediatR;
using Sky.Domain.Ports;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;

namespace Sky.Application.UseCases.PaymentMethod.Commands;

public sealed record UpdatePaymentMethodCommand(
    int PaymentMethodId,
    string MethodName) : IRequest<PaymentMethodEntity?>;

internal sealed class UpdatePaymentMethodHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodEntity?>
{
    public async Task<PaymentMethodEntity?> Handle(UpdatePaymentMethodCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentMethodEntity>().GetByIdAsync(request.PaymentMethodId, ct);
        if (entity is null) return null;

        entity.MethodName = request.MethodName;

        uow.Repository<PaymentMethodEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
