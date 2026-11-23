using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sky.Application.Ports;
using Sky.Domain.Entities;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class JwtTokenGenerator(IConfiguration config, IDateTimeProvider clock) : IJwtTokenGenerator
{
    // Nombres de claim cortos; CurrentUserService y la validación del JWT usan los mismos.
    public const string UserIdClaim = "sub";
    public const string UsernameClaim = "name";
    public const string RoleClaim = "role";
    public const string EmployeeIdClaim = "employee_id";

    public (string Token, DateTime ExpiresAtUtc) Generate(AppUser user, AppRole role)
    {
        var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Falta Jwt:Key (user-secrets o variable de entorno).");
        var issuer = config["Jwt:Issuer"] ?? "SkyPort";
        var audience = config["Jwt:Audience"] ?? "SkyPortClients";
        var minutes = config.GetValue("Jwt:ExpirationMinutes", 60);
        var now = clock.UtcNow;
        var expiresAtUtc = now.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(UserIdClaim, user.UserId.ToString(CultureInfo.InvariantCulture)),
            new(UsernameClaim, user.Username),
            new("email", user.Email),
            new(RoleClaim, role.RoleName),
            new("jti", Guid.NewGuid().ToString())
        };

        if (user.EmployeeId is int employeeId)
        {
            claims.Add(new Claim(EmployeeIdClaim, employeeId.ToString(CultureInfo.InvariantCulture)));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
