using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.PaymentStatus.Commands;
using Sky.Application.UseCases.PaymentStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/payment-statuses")]
public sealed class PaymentStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreatePaymentStatusRequest(string StatusName);
    public sealed record UpdatePaymentStatusRequest(string StatusName);

    [HttpPost]
    public async Task<IActionResult> CreatePaymentStatus(CreatePaymentStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreatePaymentStatusCommand(body.StatusName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{paymentStatusId:int}")]
    public async Task<IActionResult> DeletePaymentStatus(int paymentStatusId, CancellationToken ct)
        => await sender.Send(new DeletePaymentStatusCommand(paymentStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{paymentStatusId:int}")]
    public async Task<IActionResult> UpdatePaymentStatus(int paymentStatusId, UpdatePaymentStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdatePaymentStatusCommand(paymentStatusId, body.StatusName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{paymentStatusId:int}")]
    public async Task<IActionResult> GetPaymentStatusById(int paymentStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentStatusByIdQuery(paymentStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetPaymentStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetPaymentStatusesQuery(), ct));
}
