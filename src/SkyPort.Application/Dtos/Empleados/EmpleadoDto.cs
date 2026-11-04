namespace SkyPort.Application.Dtos.Empleados;

public sealed record EmpleadoDto(
    int EmpleadoId,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    string Cargo,
    string? Email,
    bool? Activo);
