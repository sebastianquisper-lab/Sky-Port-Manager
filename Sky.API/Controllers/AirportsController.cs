using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Airport.Commands;
using Sky.Application.UseCases.Airport.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/airports")]
public sealed class AirportsController(ISender sender) : ControllerBase
{
    public sealed record CreateAirportRequest(int CityId, string AirportName, string IataCode, string IcaoCode, string? Address, string TimeZone, decimal? Latitude, decimal? Longitude);
    public sealed record UpdateAirportRequest(int CityId, string AirportName, string IataCode, string IcaoCode, string? Address, string TimeZone, decimal? Latitude, decimal? Longitude, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateAirport(CreateAirportRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAirportCommand(body.CityId, body.AirportName, body.IataCode, body.IcaoCode, body.Address, body.TimeZone, body.Latitude, body.Longitude), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{airportId:int}")]
    public async Task<IActionResult> DeleteAirport(int airportId, CancellationToken ct)
        => await sender.Send(new DeleteAirportCommand(airportId), ct) ? NoContent() : NotFound();
    [HttpPut("{airportId:int}")]
    public async Task<IActionResult> UpdateAirport(int airportId, UpdateAirportRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAirportCommand(airportId, body.CityId, body.AirportName, body.IataCode, body.IcaoCode, body.Address, body.TimeZone, body.Latitude, body.Longitude, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{airportId:int}")]
    public async Task<IActionResult> GetAirportById(int airportId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAirportByIdQuery(airportId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAirports(CancellationToken ct)
        => Ok(await sender.Send(new GetAirportsQuery(), ct));
}
