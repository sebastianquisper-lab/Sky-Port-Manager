using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftSeat.Commands;
using Sky.Application.UseCases.AircraftSeat.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-seats")]
public sealed class AircraftSeatsController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftSeatRequest(int AircraftId, int CabinClassId, string SeatNumber, bool IsWindow, bool IsAisle, bool IsEmergencyExit);
    public sealed record UpdateAircraftSeatRequest(int AircraftId, int CabinClassId, string SeatNumber, bool IsWindow, bool IsAisle, bool IsEmergencyExit);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftSeat(CreateAircraftSeatRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftSeatCommand(body.AircraftId, body.CabinClassId, body.SeatNumber, body.IsWindow, body.IsAisle, body.IsEmergencyExit), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{aircraftSeatId:int}")]
    public async Task<IActionResult> DeleteAircraftSeat(int aircraftSeatId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftSeatCommand(aircraftSeatId), ct) ? NoContent() : NotFound();
    [HttpPut("{aircraftSeatId:int}")]
    public async Task<IActionResult> UpdateAircraftSeat(int aircraftSeatId, UpdateAircraftSeatRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftSeatCommand(aircraftSeatId, body.AircraftId, body.CabinClassId, body.SeatNumber, body.IsWindow, body.IsAisle, body.IsEmergencyExit), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{aircraftSeatId:int}")]
    public async Task<IActionResult> GetAircraftSeatById(int aircraftSeatId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftSeatByIdQuery(aircraftSeatId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftSeats(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftSeatsQuery(), ct));
}
