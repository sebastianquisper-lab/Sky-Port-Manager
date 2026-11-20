using MediatR;
using Sky.Domain.Ports;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.PaymentStatus.Commands;

public sealed record DeletePaymentStatusCommand(
    int PaymentStatusId) : IRequest<bool>;

internal sealed class DeletePaymentStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeletePaymentStatusCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentStatusEntity>().GetByIdAsync(request.PaymentStatusId, ct);
        if (entity is null) return false;

        uow.Repository<PaymentStatusEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
