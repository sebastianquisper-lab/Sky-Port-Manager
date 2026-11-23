using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftStatus.Commands;
using Sky.Application.UseCases.AircraftStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-statuses")]
public sealed class AircraftStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftStatusRequest(string StatusName, string? Description);
    public sealed record UpdateAircraftStatusRequest(string StatusName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftStatus(CreateAircraftStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftStatusCommand(body.StatusName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{aircraftStatusId:int}")]
    public async Task<IActionResult> DeleteAircraftStatus(int aircraftStatusId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftStatusCommand(aircraftStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{aircraftStatusId:int}")]
    public async Task<IActionResult> UpdateAircraftStatus(int aircraftStatusId, UpdateAircraftStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftStatusCommand(aircraftStatusId, body.StatusName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{aircraftStatusId:int}")]
    public async Task<IActionResult> GetAircraftStatusById(int aircraftStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftStatusByIdQuery(aircraftStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftStatusesQuery(), ct));
}
