using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightStatus.Commands;
using Sky.Application.UseCases.FlightStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-statuses")]
public sealed class FlightStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightStatusRequest(string StatusName, string? Description);
    public sealed record UpdateFlightStatusRequest(string StatusName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateFlightStatus(CreateFlightStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightStatusCommand(body.StatusName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{flightStatusId:int}")]
    public async Task<IActionResult> DeleteFlightStatus(int flightStatusId, CancellationToken ct)
        => await sender.Send(new DeleteFlightStatusCommand(flightStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{flightStatusId:int}")]
    public async Task<IActionResult> UpdateFlightStatus(int flightStatusId, UpdateFlightStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightStatusCommand(flightStatusId, body.StatusName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{flightStatusId:int}")]
    public async Task<IActionResult> GetFlightStatusById(int flightStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightStatusByIdQuery(flightStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightStatusesQuery(), ct));
}
