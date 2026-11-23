using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftMaintenance.Commands;
using Sky.Application.UseCases.AircraftMaintenance.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-maintenances")]
public sealed class AircraftMaintenancesController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftMaintenanceRequest(int AircraftId, int MaintenanceTypeId, int AirportId, int? TechnicianEmployeeId, string Status, DateTime StartDateUtc, DateTime? EndDateUtc, string? Notes);
    public sealed record UpdateAircraftMaintenanceRequest(int AircraftId, int MaintenanceTypeId, int AirportId, int? TechnicianEmployeeId, string Status, DateTime StartDateUtc, DateTime? EndDateUtc, string? Notes);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftMaintenance(CreateAircraftMaintenanceRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftMaintenanceCommand(body.AircraftId, body.MaintenanceTypeId, body.AirportId, body.TechnicianEmployeeId, body.Status, body.StartDateUtc, body.EndDateUtc, body.Notes), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{maintenanceId:int}")]
    public async Task<IActionResult> DeleteAircraftMaintenance(int maintenanceId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftMaintenanceCommand(maintenanceId), ct) ? NoContent() : NotFound();
    [HttpPut("{maintenanceId:int}")]
    public async Task<IActionResult> UpdateAircraftMaintenance(int maintenanceId, UpdateAircraftMaintenanceRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftMaintenanceCommand(maintenanceId, body.AircraftId, body.MaintenanceTypeId, body.AirportId, body.TechnicianEmployeeId, body.Status, body.StartDateUtc, body.EndDateUtc, body.Notes), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{maintenanceId:int}")]
    public async Task<IActionResult> GetAircraftMaintenanceById(int maintenanceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftMaintenanceByIdQuery(maintenanceId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftMaintenances(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftMaintenancesQuery(), ct));
}
