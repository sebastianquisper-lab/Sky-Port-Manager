namespace SkyPort.Application.UseCases.Rutas.Dtos;

public sealed record RutaDto(
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int DuracionMinutos);
