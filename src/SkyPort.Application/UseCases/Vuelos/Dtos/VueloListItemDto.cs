namespace SkyPort.Application.UseCases.Vuelos.Dtos;

public sealed record VueloListItemDto(
    int VueloId,
    string NumeroVuelo,
    string Ruta,
    string Aerolinea,
    DateTime FechaSalida,
    DateTime FechaLlegada,
    string Estado,
    string? Puerta,
    int ReservasConfirmadas);
