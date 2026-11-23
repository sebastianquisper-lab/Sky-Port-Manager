using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Aircraft.Commands;
using Sky.Application.UseCases.Aircraft.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircrafts")]
public sealed class AircraftsController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftRequest(int AirlineId, int ModelId, int AircraftStatusId, string RegistrationCode, string? SerialNumber, short SeatCapacity, short? ManufactureYear);
    public sealed record UpdateAircraftRequest(int AirlineId, int ModelId, int AircraftStatusId, string RegistrationCode, string? SerialNumber, short SeatCapacity, short? ManufactureYear, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateAircraft(CreateAircraftRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftCommand(body.AirlineId, body.ModelId, body.AircraftStatusId, body.RegistrationCode, body.SerialNumber, body.SeatCapacity, body.ManufactureYear), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{aircraftId:int}")]
    public async Task<IActionResult> DeleteAircraft(int aircraftId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftCommand(aircraftId), ct) ? NoContent() : NotFound();
    [HttpPut("{aircraftId:int}")]
    public async Task<IActionResult> UpdateAircraft(int aircraftId, UpdateAircraftRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftCommand(aircraftId, body.AirlineId, body.ModelId, body.AircraftStatusId, body.RegistrationCode, body.SerialNumber, body.SeatCapacity, body.ManufactureYear, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{aircraftId:int}")]
    public async Task<IActionResult> GetAircraftById(int aircraftId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftByIdQuery(aircraftId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircrafts(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftsQuery(), ct));
}
