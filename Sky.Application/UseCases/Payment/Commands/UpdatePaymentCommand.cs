using MediatR;
using Sky.Domain.Ports;
using PaymentEntity = Sky.Domain.Entities.Payment;

namespace Sky.Application.UseCases.Payment.Commands;

public sealed record UpdatePaymentCommand(
    int PaymentId,
    int BookingId,
    int PaymentMethodId,
    int PaymentStatusId,
    string CurrencyCode,
    decimal Amount,
    string? TransactionReference,
    DateTime? PaidAtUtc) : IRequest<PaymentEntity?>;

internal sealed class UpdatePaymentHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePaymentCommand, PaymentEntity?>
{
    public async Task<PaymentEntity?> Handle(UpdatePaymentCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PaymentEntity>().GetByIdAsync(request.PaymentId, ct);
        if (entity is null) return null;

        entity.BookingId = request.BookingId;
        entity.PaymentMethodId = request.PaymentMethodId;
        entity.PaymentStatusId = request.PaymentStatusId;
        entity.CurrencyCode = request.CurrencyCode;
        entity.Amount = request.Amount;
        entity.TransactionReference = request.TransactionReference;
        entity.PaidAtUtc = request.PaidAtUtc;

        uow.Repository<PaymentEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
