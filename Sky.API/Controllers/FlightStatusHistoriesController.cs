using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightStatusHistory.Commands;
using Sky.Application.UseCases.FlightStatusHistory.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-status-histories")]
public sealed class FlightStatusHistoriesController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightStatusHistoryRequest(int FlightId, int FlightStatusId, string? Notes);

    [HttpPost]
    public async Task<IActionResult> CreateFlightStatusHistory(CreateFlightStatusHistoryRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightStatusHistoryCommand(body.FlightId, body.FlightStatusId, body.Notes), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightStatusHistories(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightStatusHistoriesQuery(), ct));
    [HttpGet("{historyId:int}")]
    public async Task<IActionResult> GetFlightStatusHistoryById(int historyId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightStatusHistoryByIdQuery(historyId), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
