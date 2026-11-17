namespace Sky.Application.Ports;

/// <summary>Usuario autenticado que ejecuta la petición actual (se lee de los claims del JWT).</summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    int? EmployeeId { get; }
    string? Role { get; }
    string? IpAddress { get; }
}
