using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Refund.Commands;
using Sky.Application.UseCases.Refund.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/refunds")]
public sealed class RefundsController(ISender sender) : ControllerBase
{
    public sealed record CreateRefundRequest(int PaymentId, int? TicketId, decimal Amount, string? Reason);

    [HttpPost]
    public async Task<IActionResult> CreateRefund(CreateRefundRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateRefundCommand(body.PaymentId, body.TicketId, body.Amount, body.Reason), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpGet("{refundId:int}")]
    public async Task<IActionResult> GetRefundById(int refundId, CancellationToken ct)
    {
        var result = await sender.Send(new GetRefundByIdQuery(refundId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetRefunds(CancellationToken ct)
        => Ok(await sender.Send(new GetRefundsQuery(), ct));
}
