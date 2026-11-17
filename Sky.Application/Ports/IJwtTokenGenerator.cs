using Sky.Domain.Entities;

namespace Sky.Application.Ports;

public interface IJwtTokenGenerator
{
    /// <summary>Genera el JWT del usuario y devuelve el token junto con su fecha de expiración (UTC).</summary>
    (string Token, DateTime ExpiresAtUtc) Generate(AppUser user, AppRole role);
}
