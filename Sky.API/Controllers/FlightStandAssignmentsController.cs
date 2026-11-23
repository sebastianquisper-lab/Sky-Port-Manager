using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightStandAssignment.Commands;
using Sky.Application.UseCases.FlightStandAssignment.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-stand-assignments")]
public sealed class FlightStandAssignmentsController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightStandAssignmentRequest(int FlightId, int StandId, DateTime AssignedFromUtc, DateTime AssignedToUtc);
    public sealed record UpdateFlightStandAssignmentRequest(int FlightId, int StandId, DateTime AssignedFromUtc, DateTime AssignedToUtc);

    [HttpPost]
    public async Task<IActionResult> CreateFlightStandAssignment(CreateFlightStandAssignmentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightStandAssignmentCommand(body.FlightId, body.StandId, body.AssignedFromUtc, body.AssignedToUtc), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{assignmentId:int}")]
    public async Task<IActionResult> DeleteFlightStandAssignment(int assignmentId, CancellationToken ct)
        => await sender.Send(new DeleteFlightStandAssignmentCommand(assignmentId), ct) ? NoContent() : NotFound();
    [HttpPut("{assignmentId:int}")]
    public async Task<IActionResult> UpdateFlightStandAssignment(int assignmentId, UpdateFlightStandAssignmentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightStandAssignmentCommand(assignmentId, body.FlightId, body.StandId, body.AssignedFromUtc, body.AssignedToUtc), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{assignmentId:int}")]
    public async Task<IActionResult> GetFlightStandAssignmentById(int assignmentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightStandAssignmentByIdQuery(assignmentId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightStandAssignments(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightStandAssignmentsQuery(), ct));
}
