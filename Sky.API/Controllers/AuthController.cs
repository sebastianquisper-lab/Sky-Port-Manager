using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.DTOs;
using Sky.Application.UseCases.Auth.Commands;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    public sealed record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequest request, CancellationToken ct)
        => Ok(await sender.Send(new LoginCommand(request.Username, request.Password), ct));
}
