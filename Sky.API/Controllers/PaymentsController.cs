using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Payment.Commands;
using Sky.Application.UseCases.Payment.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/payments")]
public sealed class PaymentsController(ISender sender) : ControllerBase
{
    public sealed record CreatePaymentRequest(int BookingId, int PaymentMethodId, string CurrencyCode, decimal Amount, string? PaymentToken);
    public sealed record UpdatePaymentRequest(int BookingId, int PaymentMethodId, int PaymentStatusId, string CurrencyCode, decimal Amount, string? TransactionReference, DateTime? PaidAtUtc);

    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreatePaymentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreatePaymentCommand(body.BookingId, body.PaymentMethodId, body.CurrencyCode, body.Amount, body.PaymentToken), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{paymentId:int}")]
    public async Task<IActionResult> DeletePayment(int paymentId, CancellationToken ct)
        => await sender.Send(new DeletePaymentCommand(paymentId), ct) ? NoContent() : NotFound();
    [HttpPut("{paymentId:int}")]
    public async Task<IActionResult> UpdatePayment(int paymentId, UpdatePaymentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdatePaymentCommand(paymentId, body.BookingId, body.PaymentMethodId, body.PaymentStatusId, body.CurrencyCode, body.Amount, body.TransactionReference, body.PaidAtUtc), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{paymentId:int}")]
    public async Task<IActionResult> GetPaymentById(int paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(paymentId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetPayments(CancellationToken ct)
        => Ok(await sender.Send(new GetPaymentsQuery(), ct));
}
