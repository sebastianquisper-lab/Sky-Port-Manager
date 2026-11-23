using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.EmployeeRole.Commands;
using Sky.Application.UseCases.EmployeeRole.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/employee-roles")]
public sealed class EmployeeRolesController(ISender sender) : ControllerBase
{
    public sealed record CreateEmployeeRoleRequest(string RoleName, string? Description);
    public sealed record UpdateEmployeeRoleRequest(string RoleName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeRole(CreateEmployeeRoleRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateEmployeeRoleCommand(body.RoleName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{employeeRoleId:int}")]
    public async Task<IActionResult> DeleteEmployeeRole(int employeeRoleId, CancellationToken ct)
        => await sender.Send(new DeleteEmployeeRoleCommand(employeeRoleId), ct) ? NoContent() : NotFound();
    [HttpPut("{employeeRoleId:int}")]
    public async Task<IActionResult> UpdateEmployeeRole(int employeeRoleId, UpdateEmployeeRoleRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateEmployeeRoleCommand(employeeRoleId, body.RoleName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{employeeRoleId:int}")]
    public async Task<IActionResult> GetEmployeeRoleById(int employeeRoleId, CancellationToken ct)
    {
        var result = await sender.Send(new GetEmployeeRoleByIdQuery(employeeRoleId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetEmployeeRoles(CancellationToken ct)
        => Ok(await sender.Send(new GetEmployeeRolesQuery(), ct));
}
