namespace SkyPort.Application.UseCases.Pasajeros.Dtos;

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
