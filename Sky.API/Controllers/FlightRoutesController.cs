using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightRoute.Commands;
using Sky.Application.UseCases.FlightRoute.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-routes")]
public sealed class FlightRoutesController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightRouteRequest(int OriginAirportId, int DestinationAirportId, decimal DistanceKm, int EstimatedDurationMinutes);
    public sealed record UpdateFlightRouteRequest(int OriginAirportId, int DestinationAirportId, decimal DistanceKm, int EstimatedDurationMinutes, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateFlightRoute(CreateFlightRouteRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightRouteCommand(body.OriginAirportId, body.DestinationAirportId, body.DistanceKm, body.EstimatedDurationMinutes), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{routeId:int}")]
    public async Task<IActionResult> DeleteFlightRoute(int routeId, CancellationToken ct)
        => await sender.Send(new DeleteFlightRouteCommand(routeId), ct) ? NoContent() : NotFound();
    [HttpPut("{routeId:int}")]
    public async Task<IActionResult> UpdateFlightRoute(int routeId, UpdateFlightRouteRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightRouteCommand(routeId, body.OriginAirportId, body.DestinationAirportId, body.DistanceKm, body.EstimatedDurationMinutes, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{routeId:int}")]
    public async Task<IActionResult> GetFlightRouteById(int routeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightRouteByIdQuery(routeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightRoutes(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightRoutesQuery(), ct));
}
