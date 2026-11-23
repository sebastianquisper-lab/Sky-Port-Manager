using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Airline.Commands;
using Sky.Application.UseCases.Airline.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/airlines")]
public sealed class AirlinesController(ISender sender) : ControllerBase
{
    public sealed record CreateAirlineRequest(int CountryId, string AirlineName, string IataCode, string IcaoCode, string? ContactEmail, string? ContactPhone);
    public sealed record UpdateAirlineRequest(int CountryId, string AirlineName, string IataCode, string IcaoCode, string? ContactEmail, string? ContactPhone, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateAirline(CreateAirlineRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAirlineCommand(body.CountryId, body.AirlineName, body.IataCode, body.IcaoCode, body.ContactEmail, body.ContactPhone), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{airlineId:int}")]
    public async Task<IActionResult> DeleteAirline(int airlineId, CancellationToken ct)
        => await sender.Send(new DeleteAirlineCommand(airlineId), ct) ? NoContent() : NotFound();
    [HttpPut("{airlineId:int}")]
    public async Task<IActionResult> UpdateAirline(int airlineId, UpdateAirlineRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAirlineCommand(airlineId, body.CountryId, body.AirlineName, body.IataCode, body.IcaoCode, body.ContactEmail, body.ContactPhone, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{airlineId:int}")]
    public async Task<IActionResult> GetAirlineById(int airlineId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAirlineByIdQuery(airlineId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAirlines(CancellationToken ct)
        => Ok(await sender.Send(new GetAirlinesQuery(), ct));
}
