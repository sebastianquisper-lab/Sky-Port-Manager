using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.FlightCrewAssignment.Commands;
using Sky.Application.UseCases.FlightCrewAssignment.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-crew-assignments")]
public sealed class FlightCrewAssignmentsController(ISender sender) : ControllerBase
{
    public sealed record CreateFlightCrewAssignmentRequest(int FlightId, int EmployeeId, string CrewPosition);
    public sealed record UpdateFlightCrewAssignmentRequest(string CrewPosition);

    [HttpPost]
    public async Task<IActionResult> CreateFlightCrewAssignment(CreateFlightCrewAssignmentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFlightCrewAssignmentCommand(body.FlightId, body.EmployeeId, body.CrewPosition), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{flightId:int}/{employeeId:int}")]
    public async Task<IActionResult> DeleteFlightCrewAssignment(int flightId, int employeeId, CancellationToken ct)
        => await sender.Send(new DeleteFlightCrewAssignmentCommand(flightId, employeeId), ct) ? NoContent() : NotFound();
    [HttpPut("{flightId:int}/{employeeId:int}")]
    public async Task<IActionResult> UpdateFlightCrewAssignment(int flightId, int employeeId, UpdateFlightCrewAssignmentRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFlightCrewAssignmentCommand(flightId, employeeId, body.CrewPosition), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{flightId:int}/{employeeId:int}")]
    public async Task<IActionResult> GetFlightCrewAssignmentById(int flightId, int employeeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFlightCrewAssignmentByIdQuery(flightId, employeeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlightCrewAssignments(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightCrewAssignmentsQuery(), ct));
}
