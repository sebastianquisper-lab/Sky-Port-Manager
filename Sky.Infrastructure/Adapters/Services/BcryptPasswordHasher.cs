using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // El hash guardado en la BD no es BCrypt (por ejemplo, un dato de prueba): no autentica.
            return false;
        }
    }
}
