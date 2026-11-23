using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public int? UserId => ReadInt(JwtTokenGenerator.UserIdClaim);

    public int? EmployeeId => ReadInt(JwtTokenGenerator.EmployeeIdClaim);

    public string? Role => User?.FindFirstValue(JwtTokenGenerator.RoleClaim);

    public string? IpAddress => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private int? ReadInt(string claimType)
        => int.TryParse(User?.FindFirstValue(claimType), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
}
