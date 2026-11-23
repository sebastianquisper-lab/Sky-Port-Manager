using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BaggageStatus.Commands;
using Sky.Application.UseCases.BaggageStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/baggage-statuses")]
public sealed class BaggageStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreateBaggageStatusRequest(string StatusName);
    public sealed record UpdateBaggageStatusRequest(string StatusName);

    [HttpPost]
    public async Task<IActionResult> CreateBaggageStatus(CreateBaggageStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBaggageStatusCommand(body.StatusName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{baggageStatusId:int}")]
    public async Task<IActionResult> DeleteBaggageStatus(int baggageStatusId, CancellationToken ct)
        => await sender.Send(new DeleteBaggageStatusCommand(baggageStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{baggageStatusId:int}")]
    public async Task<IActionResult> UpdateBaggageStatus(int baggageStatusId, UpdateBaggageStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBaggageStatusCommand(baggageStatusId, body.StatusName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{baggageStatusId:int}")]
    public async Task<IActionResult> GetBaggageStatusById(int baggageStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBaggageStatusByIdQuery(baggageStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBaggageStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetBaggageStatusesQuery(), ct));
}
