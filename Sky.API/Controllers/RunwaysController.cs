using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Runway.Commands;
using Sky.Application.UseCases.Runway.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/runways")]
public sealed class RunwaysController(ISender sender) : ControllerBase
{
    public sealed record CreateRunwayRequest(int AirportId, string RunwayCode, int LengthM, string? SurfaceType);
    public sealed record UpdateRunwayRequest(int AirportId, string RunwayCode, int LengthM, string? SurfaceType, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateRunway(CreateRunwayRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateRunwayCommand(body.AirportId, body.RunwayCode, body.LengthM, body.SurfaceType), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{runwayId:int}")]
    public async Task<IActionResult> DeleteRunway(int runwayId, CancellationToken ct)
        => await sender.Send(new DeleteRunwayCommand(runwayId), ct) ? NoContent() : NotFound();
    [HttpPut("{runwayId:int}")]
    public async Task<IActionResult> UpdateRunway(int runwayId, UpdateRunwayRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateRunwayCommand(runwayId, body.AirportId, body.RunwayCode, body.LengthM, body.SurfaceType, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{runwayId:int}")]
    public async Task<IActionResult> GetRunwayById(int runwayId, CancellationToken ct)
    {
        var result = await sender.Send(new GetRunwayByIdQuery(runwayId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetRunways(CancellationToken ct)
        => Ok(await sender.Send(new GetRunwaysQuery(), ct));
}
