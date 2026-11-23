using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.CheckInCounter.Commands;
using Sky.Application.UseCases.CheckInCounter.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/check-in-counters")]
public sealed class CheckInCountersController(ISender sender) : ControllerBase
{
    public sealed record CreateCheckInCounterRequest(int TerminalId, string CounterCode);
    public sealed record UpdateCheckInCounterRequest(int TerminalId, string CounterCode, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateCheckInCounter(CreateCheckInCounterRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCheckInCounterCommand(body.TerminalId, body.CounterCode), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{counterId:int}")]
    public async Task<IActionResult> DeleteCheckInCounter(int counterId, CancellationToken ct)
        => await sender.Send(new DeleteCheckInCounterCommand(counterId), ct) ? NoContent() : NotFound();
    [HttpPut("{counterId:int}")]
    public async Task<IActionResult> UpdateCheckInCounter(int counterId, UpdateCheckInCounterRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCheckInCounterCommand(counterId, body.TerminalId, body.CounterCode, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{counterId:int}")]
    public async Task<IActionResult> GetCheckInCounterById(int counterId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCheckInCounterByIdQuery(counterId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCheckInCounters(CancellationToken ct)
        => Ok(await sender.Send(new GetCheckInCountersQuery(), ct));
}
