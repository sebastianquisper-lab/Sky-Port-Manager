using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightDelay.Commands;
using Sky.Application.UseCases.FlightDelay.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-delays")]
public sealed class FlightDelaysController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightDelayRequest(int FlightId, int DelayReasonId, int DelayMinutes, string? Notes);
    public sealed record UpdateFlightDelayRequest(int FlightId, int DelayReasonId, int DelayMinutes, string? Notes);

    [HttpPost]
    public async Task<IActionResult> CreateFlightDelay(CreateFlightDelayRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightDelayCommand(body.FlightId, body.DelayReasonId, body.DelayMinutes, body.Notes), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{flightDelayId:int}")]
    public async Task<IActionResult> DeleteFlightDelay(int flightDelayId, CancellationToken ct)
        => await sender.Send(new DeleteFlightDelayCommand(flightDelayId), ct) ? NoContent() : NotFound();
    [HttpPut("{flightDelayId:int}")]
    public async Task<IActionResult> UpdateFlightDelay(int flightDelayId, UpdateFlightDelayRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightDelayCommand(flightDelayId, body.FlightId, body.DelayReasonId, body.DelayMinutes, body.Notes), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{flightDelayId:int}")]
    public async Task<IActionResult> GetFlightDelayById(int flightDelayId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightDelayByIdQuery(flightDelayId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightDelays(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightDelaysQuery(), ct));
}
