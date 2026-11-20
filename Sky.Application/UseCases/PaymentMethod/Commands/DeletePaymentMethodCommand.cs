using MediatR;
using Sky.Domain.Ports;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;

namespace Sky.Application.UseCases.PaymentMethod.Commands;

public sealed record DeletePaymentMethodCommand(
    int PaymentMethodId) : IRequest<bool>;

internal sealed class DeletePaymentMethodHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePaymentMethodCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentMethodEntity>().GetByIdAsync(request.PaymentMethodId, ct);
        if (entity is null) return false;

        uow.Repository<PaymentMethodEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
