namespace SkyPort.Application.Dtos.Pasajeros;

public sealed record PasajeroDto(
    int PasajeroId,
    string TipoDocumento,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    DateOnly FechaNacimiento,
    string Nacionalidad,
    string? Email,
    string? Telefono);
