using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AirlineAirport.Commands;
using Sky.Application.UseCases.AirlineAirport.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/airline-airports")]
public sealed class AirlineAirportsController(ISender sender) : ControllerBase
{
    public sealed record CreateAirlineAirportRequest(int AirlineId, int AirportId, DateOnly SinceDate, bool IsHub);
    public sealed record UpdateAirlineAirportRequest(DateOnly SinceDate, bool IsHub);

    [HttpPost]
    public async Task<IActionResult> CreateAirlineAirport(CreateAirlineAirportRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAirlineAirportCommand(body.AirlineId, body.AirportId, body.SinceDate, body.IsHub), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{airlineId:int}/{airportId:int}")]
    public async Task<IActionResult> DeleteAirlineAirport(int airlineId, int airportId, CancellationToken ct)
        => await sender.Send(new DeleteAirlineAirportCommand(airlineId, airportId), ct) ? NoContent() : NotFound();
    [HttpPut("{airlineId:int}/{airportId:int}")]
    public async Task<IActionResult> UpdateAirlineAirport(int airlineId, int airportId, UpdateAirlineAirportRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAirlineAirportCommand(airlineId, airportId, body.SinceDate, body.IsHub), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{airlineId:int}/{airportId:int}")]
    public async Task<IActionResult> GetAirlineAirportById(int airlineId, int airportId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAirlineAirportByIdQuery(airlineId, airportId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAirlineAirports(CancellationToken ct)
        => Ok(await sender.Send(new GetAirlineAirportsQuery(), ct));
}
