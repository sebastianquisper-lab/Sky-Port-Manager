using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AppRole.Commands;
using Sky.Application.UseCases.AppRole.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/app-roles")]
public sealed class AppRolesController(ISender sender) : ControllerBase
{
    public sealed record CreateAppRoleRequest(string RoleName, string? Description);
    public sealed record UpdateAppRoleRequest(string RoleName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateAppRole(CreateAppRoleRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAppRoleCommand(body.RoleName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{appRoleId:int}")]
    public async Task<IActionResult> DeleteAppRole(int appRoleId, CancellationToken ct)
        => await sender.Send(new DeleteAppRoleCommand(appRoleId), ct) ? NoContent() : NotFound();
    [HttpPut("{appRoleId:int}")]
    public async Task<IActionResult> UpdateAppRole(int appRoleId, UpdateAppRoleRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAppRoleCommand(appRoleId, body.RoleName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{appRoleId:int}")]
    public async Task<IActionResult> GetAppRoleById(int appRoleId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAppRoleByIdQuery(appRoleId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAppRoles(CancellationToken ct)
        => Ok(await sender.Send(new GetAppRolesQuery(), ct));
}
