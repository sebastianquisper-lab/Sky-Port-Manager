namespace SkyPort.Application.Dtos.Aerolineas;

public sealed record AerolineaDto(
    int AerolineaId,
    string Nombre,
    string CodigoIata,
    string Pais,
    bool Activa);
