using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Flight.Commands;
using Sky.Application.UseCases.Flight.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flights")]
public sealed class FlightsController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightRequest(int AirlineId, int AircraftId, int RouteId, int FlightStatusId, string FlightNumber, DateTime ScheduledDepartureUtc, DateTime ScheduledArrivalUtc, DateTime? ActualDepartureUtc, DateTime? ActualArrivalUtc, int? DepartureGateId, int? ArrivalGateId);
    public sealed record UpdateFlightRequest(int AirlineId, int AircraftId, int RouteId, int FlightStatusId, string FlightNumber, DateTime ScheduledDepartureUtc, DateTime ScheduledArrivalUtc, DateTime? ActualDepartureUtc, DateTime? ActualArrivalUtc, int? DepartureGateId, int? ArrivalGateId);

    [HttpPost]
    public async Task<IActionResult> CreateFlight(CreateFlightRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightCommand(body.AirlineId, body.AircraftId, body.RouteId, body.FlightStatusId, body.FlightNumber, body.ScheduledDepartureUtc, body.ScheduledArrivalUtc, body.ActualDepartureUtc, body.ActualArrivalUtc, body.DepartureGateId, body.ArrivalGateId), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> DeleteFlight(int flightId, CancellationToken ct)
        => await sender.Send(new DeleteFlightCommand(flightId), ct) ? NoContent() : NotFound();
    [HttpPut("{flightId:int}")]
    public async Task<IActionResult> UpdateFlight(int flightId, UpdateFlightRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightCommand(flightId, body.AirlineId, body.AircraftId, body.RouteId, body.FlightStatusId, body.FlightNumber, body.ScheduledDepartureUtc, body.ScheduledArrivalUtc, body.ActualDepartureUtc, body.ActualArrivalUtc, body.DepartureGateId, body.ArrivalGateId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{flightId:int}")]
    public async Task<IActionResult> GetFlightById(int flightId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightByIdQuery(flightId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlights(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightsQuery(), ct));
}
