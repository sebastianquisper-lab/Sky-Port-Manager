using MediatR;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using PaymentEntity = Sky.Domain.Entities.Payment;
using PaymentStatusEntity = Sky.Domain.Entities.PaymentStatus;
using RefundEntity = Sky.Domain.Entities.Refund;
using TicketEntity = Sky.Domain.Entities.Ticket;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.Refund.Commands;

public sealed record CreateRefundCommand(
    int PaymentId,
    int? TicketId,
    decimal Amount,
    string? Reason) : IRequest<RefundEntity>;

internal sealed class CreateRefundHandler(
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICurrentUserService currentUser,
    IPaymentGateway gateway)
    : IRequestHandler<CreateRefundCommand, RefundEntity>
{
    private const string CompletedPaymentStatus = "COMPLETED";
    private const string RefundedTicketStatus = "REFUNDED";

    public async Task<RefundEntity> Handle(CreateRefundCommand request, CancellationToken ct)
    {
        var payment = await uow.Repository<PaymentEntity>().GetByIdAsync(request.PaymentId, ct)
            ?? throw new KeyNotFoundException($"El pago {request.PaymentId} no existe.");

        var completed = await uow.Repository<PaymentStatusEntity>()
            .FirstOrDefaultAsync(s => s.StatusName == CompletedPaymentStatus, ct)
            ?? throw new KeyNotFoundException($"El estado de pago '{CompletedPaymentStatus}' no existe.");

        if (payment.PaymentStatusId != completed.PaymentStatusId || string.IsNullOrWhiteSpace(payment.TransactionReference))
        {
            throw new InvalidOperationException("Solo se pueden reembolsar pagos completados.");
        }

        // Regla que la BD no cubre: la suma de reembolsos no puede superar lo pagado.
        var previousRefunds = await uow.Repository<RefundEntity>()
            .FindAsync(r => r.PaymentId == request.PaymentId, ct);

        var alreadyRefunded = previousRefunds.Sum(r => r.Amount);
        if (alreadyRefunded + request.Amount > payment.Amount)
        {
            throw new InvalidOperationException(
                $"El reembolso excede el pago: pagado {payment.Amount}, ya reembolsado {alreadyRefunded}.");
        }

        var accepted = await gateway.RefundAsync(
            payment.TransactionReference, request.Amount, payment.CurrencyCode, ct);

        if (!accepted)
        {
            throw new InvalidOperationException("La pasarela rechazó el reembolso.");
        }

        var entity = new RefundEntity
        {
            PaymentId = request.PaymentId,
            TicketId = request.TicketId,
            ProcessedByEmployeeId = currentUser.EmployeeId,
            Amount = request.Amount,
            Reason = request.Reason,
            RefundDateUtc = clock.UtcNow
        };

        await uow.Repository<RefundEntity>().AddAsync(entity, ct);

        // Si el reembolso es de un boleto, queda REFUNDED y libera su asiento (el trigger de capacidad lo ignora).
        if (request.TicketId is int ticketId)
        {
            var ticket = await uow.Repository<TicketEntity>().GetByIdAsync(ticketId, ct)
                ?? throw new KeyNotFoundException($"El boleto {ticketId} no existe.");

            var refunded = await uow.Repository<TicketStatusEntity>()
                .FirstOrDefaultAsync(s => s.StatusName == RefundedTicketStatus, ct)
                ?? throw new KeyNotFoundException($"El estado de boleto '{RefundedTicketStatus}' no existe.");

            ticket.TicketStatusId = refunded.TicketStatusId;
            uow.Repository<TicketEntity>().Update(ticket);
        }

        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
