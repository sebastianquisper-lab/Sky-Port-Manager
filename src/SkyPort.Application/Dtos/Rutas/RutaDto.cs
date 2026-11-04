namespace SkyPort.Application.Dtos.Rutas;

public sealed record RutaDto(
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int DuracionMinutos);
