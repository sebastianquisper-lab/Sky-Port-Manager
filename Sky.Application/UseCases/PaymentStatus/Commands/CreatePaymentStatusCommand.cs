using MediatR;
using Sky.Domain.Ports;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.PaymentStatus.Commands;

public sealed record CreatePaymentStatusCommand(
    string StatusName) : IRequest<PaymentStatusEntity>;

internal sealed class CreatePaymentStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreatePaymentStatusCommand, PaymentStatusEntity>
{
    public async Task<PaymentStatusEntity> Handle(CreatePaymentStatusCommand request, CancellationToken ct)
    {
        var entity = new PaymentStatusEntity
        {
            StatusName = request.StatusName
        };

        await uow.Repository<PaymentStatusEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
