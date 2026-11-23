using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.DelayReason.Commands;
using Sky.Application.UseCases.DelayReason.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/delay-reasons")]
public sealed class DelayReasonsController(ISender sender) : ControllerBase
{
    public sealed record CreateDelayReasonRequest(string ReasonCode, string Description);
    public sealed record UpdateDelayReasonRequest(string ReasonCode, string Description);

    [HttpPost]
    public async Task<IActionResult> CreateDelayReason(CreateDelayReasonRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateDelayReasonCommand(body.ReasonCode, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{delayReasonId:int}")]
    public async Task<IActionResult> DeleteDelayReason(int delayReasonId, CancellationToken ct)
        => await sender.Send(new DeleteDelayReasonCommand(delayReasonId), ct) ? NoContent() : NotFound();
    [HttpPut("{delayReasonId:int}")]
    public async Task<IActionResult> UpdateDelayReason(int delayReasonId, UpdateDelayReasonRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateDelayReasonCommand(delayReasonId, body.ReasonCode, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{delayReasonId:int}")]
    public async Task<IActionResult> GetDelayReasonById(int delayReasonId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDelayReasonByIdQuery(delayReasonId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetDelayReasons(CancellationToken ct)
        => Ok(await sender.Send(new GetDelayReasonsQuery(), ct));
}
