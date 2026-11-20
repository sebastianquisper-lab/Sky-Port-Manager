using MediatR;
using Sky.Domain.Ports;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.PaymentStatus.Commands;

public sealed record UpdatePaymentStatusCommand(
    int PaymentStatusId,
    string StatusName) : IRequest<PaymentStatusEntity?>;

internal sealed class UpdatePaymentStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePaymentStatusCommand, PaymentStatusEntity?>
{
    public async Task<PaymentStatusEntity?> Handle(UpdatePaymentStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentStatusEntity>().GetByIdAsync(request.PaymentStatusId, ct);
        if (entity is null) return null;

        entity.StatusName = request.StatusName;

        uow.Repository<PaymentStatusEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
