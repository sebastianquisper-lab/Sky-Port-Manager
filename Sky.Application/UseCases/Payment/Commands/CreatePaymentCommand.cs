using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using BookingEntity = Sky.Domain.Entities.Booking;
using PaymentEntity = Sky.Domain.Entities.Payment;
using PaymentMethodEntity = Sky.Domain.Entities.PaymentMethod;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;

namespace Sky.Application.UseCases.Payment.Commands;

/// <param name="PaymentToken">Token de la tarjeta/billetera que entrega el front (nunca el número de tarjeta).</param>
public sealed record CreatePaymentCommand(
    int BookingId,
    int PaymentMethodId,
    string CurrencyCode,
    decimal Amount,
    string? PaymentToken) : IRequest<PaymentEntity>;

internal sealed class CreatePaymentHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    IPaymentGateway gateway)
    : IRequestHandler<CreatePaymentCommand, PaymentEntity>
{
    private const string CompletedStatus = "COMPLETED";
    private const string FailedStatus = "FAILED";

    public async Task<PaymentEntity> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var booking = await uow.Repository<BookingEntity>().GetByIdAsync(request.BookingId, ct)
            ?? throw new KeyNotFoundException($"La reserva {request.BookingId} no existe.");

        var method = await uow.Repository<PaymentMethodEntity>().GetByIdAsync(request.PaymentMethodId, ct)
            ?? throw new KeyNotFoundException($"El método de pago {request.PaymentMethodId} no existe.");

        var transactionReference = await gateway.ChargeAsync(
            request.Amount,
            request.CurrencyCode,
            $"Reserva {booking.BookingCode}",
            booking.ContactEmail,
            method.MethodName,
            request.PaymentToken,
            ct);

        // Se guarda siempre el intento: COMPLETED si cobró, FAILED si fue rechazado.
        var succeeded = !string.IsNullOrWhiteSpace(transactionReference);
        var statusName = succeeded ? CompletedStatus : FailedStatus;

        var status = await uow.Repository<PaymentStatusEntity>()
            .FirstOrDefaultAsync(s => s.StatusName == statusName, ct)
            ?? throw new KeyNotFoundException($"El estado de pago '{statusName}' no existe.");

        var now = clock.UtcNow;

        var entity = new PaymentEntity
        {
            BookingId = request.BookingId,
            PaymentMethodId = request.PaymentMethodId,
            PaymentStatusId = status.PaymentStatusId,
            CurrencyCode = request.CurrencyCode,
            Amount = request.Amount,
            TransactionReference = transactionReference,
            CreatedAtUtc = now,
            PaidAtUtc = succeeded ? now : null
        };

        await uow.Repository<PaymentEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
