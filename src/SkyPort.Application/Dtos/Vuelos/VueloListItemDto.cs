namespace SkyPort.Application.Dtos.Vuelos;

public sealed record VueloListItemDto(
    int VueloId,
    string NumeroVuelo,
    string Ruta,
    string MatriculaAeronave,
    string? Puerta,
    DateTime FechaSalida,
    DateTime FechaLlegada,
    string Estado);
