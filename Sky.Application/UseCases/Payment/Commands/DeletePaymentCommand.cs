using MediatR;
using Sky.Domain.Ports;
using PaymentEntity = Sky.Domain.Entities.Payment;

namespace Sky.Application.UseCases.Payment.Commands;

public sealed record DeletePaymentCommand(
    int PaymentId) : IRequest<bool>;

internal sealed class DeletePaymentHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePaymentCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentEntity>().GetByIdAsync(request.PaymentId, ct);
        if (entity is null) return false;

        uow.Repository<PaymentEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
