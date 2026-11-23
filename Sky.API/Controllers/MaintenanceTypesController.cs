using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.MaintenanceType.Commands;
using Sky.Application.UseCases.MaintenanceType.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/maintenance-types")]
public sealed class MaintenanceTypesController(ISender sender) : ControllerBase
{
    public sealed record CreateMaintenanceTypeRequest(string TypeName, string? Description);
    public sealed record UpdateMaintenanceTypeRequest(string TypeName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateMaintenanceType(CreateMaintenanceTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateMaintenanceTypeCommand(body.TypeName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{maintenanceTypeId:int}")]
    public async Task<IActionResult> DeleteMaintenanceType(int maintenanceTypeId, CancellationToken ct)
        => await sender.Send(new DeleteMaintenanceTypeCommand(maintenanceTypeId), ct) ? NoContent() : NotFound();
    [HttpPut("{maintenanceTypeId:int}")]
    public async Task<IActionResult> UpdateMaintenanceType(int maintenanceTypeId, UpdateMaintenanceTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateMaintenanceTypeCommand(maintenanceTypeId, body.TypeName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{maintenanceTypeId:int}")]
    public async Task<IActionResult> GetMaintenanceTypeById(int maintenanceTypeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetMaintenanceTypeByIdQuery(maintenanceTypeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetMaintenanceTypes(CancellationToken ct)
        => Ok(await sender.Send(new GetMaintenanceTypesQuery(), ct));
}
