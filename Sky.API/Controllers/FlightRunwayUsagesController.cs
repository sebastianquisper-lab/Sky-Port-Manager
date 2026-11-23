using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightRunwayUsage.Commands;
using Sky.Application.UseCases.FlightRunwayUsage.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-runway-usages")]
public sealed class FlightRunwayUsagesController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightRunwayUsageRequest(int FlightId, int RunwayId, string Operation, DateTime UsedAtUtc);
    public sealed record UpdateFlightRunwayUsageRequest(int FlightId, int RunwayId, string Operation, DateTime UsedAtUtc);

    [HttpPost]
    public async Task<IActionResult> CreateFlightRunwayUsage(CreateFlightRunwayUsageRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightRunwayUsageCommand(body.FlightId, body.RunwayId, body.Operation, body.UsedAtUtc), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{usageId:int}")]
    public async Task<IActionResult> DeleteFlightRunwayUsage(int usageId, CancellationToken ct)
        => await sender.Send(new DeleteFlightRunwayUsageCommand(usageId), ct) ? NoContent() : NotFound();
    [HttpPut("{usageId:int}")]
    public async Task<IActionResult> UpdateFlightRunwayUsage(int usageId, UpdateFlightRunwayUsageRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightRunwayUsageCommand(usageId, body.FlightId, body.RunwayId, body.Operation, body.UsedAtUtc), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{usageId:int}")]
    public async Task<IActionResult> GetFlightRunwayUsageById(int usageId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightRunwayUsageByIdQuery(usageId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightRunwayUsages(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightRunwayUsagesQuery(), ct));
}
