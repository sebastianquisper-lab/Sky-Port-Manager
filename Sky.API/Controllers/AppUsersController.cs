using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AppUser.Commands;
using Sky.Application.UseCases.AppUser.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/app-users")]
public sealed class AppUsersController(ISender sender) : ControllerBase
{
    public sealed record CreateAppUserRequest(int AppRoleId, int? EmployeeId, string Username, string Email, string Password);
    public sealed record UpdateAppUserRequest(int AppRoleId, int? EmployeeId, string Username, string Email, bool IsActive, string? NewPassword);

    [HttpPost]
    public async Task<IActionResult> CreateAppUser(CreateAppUserRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAppUserCommand(body.AppRoleId, body.EmployeeId, body.Username, body.Email, body.Password), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteAppUser(int userId, CancellationToken ct)
        => await sender.Send(new DeleteAppUserCommand(userId), ct) ? NoContent() : NotFound();
    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateAppUser(int userId, UpdateAppUserRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAppUserCommand(userId, body.AppRoleId, body.EmployeeId, body.Username, body.Email, body.IsActive, body.NewPassword), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetAppUserById(int userId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAppUserByIdQuery(userId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAppUsers(CancellationToken ct)
        => Ok(await sender.Send(new GetAppUsersQuery(), ct));
}
