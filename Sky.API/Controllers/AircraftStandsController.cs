using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftStand.Commands;
using Sky.Application.UseCases.AircraftStand.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-stands")]
public sealed class AircraftStandsController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftStandRequest(int AirportId, string StandCode, bool IsRemote);
    public sealed record UpdateAircraftStandRequest(int AirportId, string StandCode, bool IsRemote, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftStand(CreateAircraftStandRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftStandCommand(body.AirportId, body.StandCode, body.IsRemote), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{standId:int}")]
    public async Task<IActionResult> DeleteAircraftStand(int standId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftStandCommand(standId), ct) ? NoContent() : NotFound();
    [HttpPut("{standId:int}")]
    public async Task<IActionResult> UpdateAircraftStand(int standId, UpdateAircraftStandRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftStandCommand(standId, body.AirportId, body.StandCode, body.IsRemote, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{standId:int}")]
    public async Task<IActionResult> GetAircraftStandById(int standId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftStandByIdQuery(standId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftStands(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftStandsQuery(), ct));
}
