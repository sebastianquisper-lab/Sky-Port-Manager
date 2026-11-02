namespace SkyPort.Application.UseCases.Vuelos.Dtos;

public sealed record VueloDetailDto(
    int VueloId,
    string NumeroVuelo,
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int AeronaveId,
    string Matricula,
    string Modelo,
    int AerolineaId,
    string Aerolinea,
    int Capacidad,
    int? PuertaId,
    string? Puerta,
    string? Terminal,
    DateTime FechaSalida,
    DateTime FechaLlegada,
    string Estado,
    int ReservasConfirmadas);
