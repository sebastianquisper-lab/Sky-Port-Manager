namespace SkyPort.Application.Dtos.Vuelos;

public sealed record VueloDetailDto(
    int VueloId,
    string NumeroVuelo,
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int AeronaveId,
    string MatriculaAeronave,
    string ModeloAeronave,
    int? PuertaId,
    string? CodigoPuerta,
    DateTime FechaSalida,
    DateTime FechaLlegada,
    string Estado);
