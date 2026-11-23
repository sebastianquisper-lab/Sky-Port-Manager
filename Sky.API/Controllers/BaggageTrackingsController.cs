using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BaggageTracking.Commands;
using Sky.Application.UseCases.BaggageTracking.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/baggage-trackings")]
public sealed class BaggageTrackingsController(ISender sender) : ControllerBase
{
    public sealed record CreateBaggageTrackingRequest(int BaggageId, int BaggageStatusId, int? AirportId, int? EmployeeId, string? LocationDescription);
    public sealed record UpdateBaggageTrackingRequest(int BaggageId, int BaggageStatusId, int? AirportId, int? EmployeeId, string? LocationDescription);

    [HttpPost]
    public async Task<IActionResult> CreateBaggageTracking(CreateBaggageTrackingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBaggageTrackingCommand(body.BaggageId, body.BaggageStatusId, body.AirportId, body.EmployeeId, body.LocationDescription), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{trackingId:int}")]
    public async Task<IActionResult> DeleteBaggageTracking(int trackingId, CancellationToken ct)
        => await sender.Send(new DeleteBaggageTrackingCommand(trackingId), ct) ? NoContent() : NotFound();
    [HttpPut("{trackingId:int}")]
    public async Task<IActionResult> UpdateBaggageTracking(int trackingId, UpdateBaggageTrackingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBaggageTrackingCommand(trackingId, body.BaggageId, body.BaggageStatusId, body.AirportId, body.EmployeeId, body.LocationDescription), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{trackingId:int}")]
    public async Task<IActionResult> GetBaggageTrackingById(int trackingId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBaggageTrackingByIdQuery(trackingId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBaggageTrackings(CancellationToken ct)
        => Ok(await sender.Send(new GetBaggageTrackingsQuery(), ct));
}
