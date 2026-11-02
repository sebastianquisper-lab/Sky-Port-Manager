namespace SkyPort.Application.UseCases.Vuelos.Dtos;

public sealed record VueloFilter(
    DateTime? Desde = null,
    DateTime? Hasta = null,
    string? Estado = null,
    int? RutaId = null,
    int? AerolineaId = null,
    string? NumeroVuelo = null);
