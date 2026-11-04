namespace SkyPort.Application.Dtos.Vuelos;

public sealed record VueloLookupsDto(
    IReadOnlyList<RutaOpcionDto> Rutas,
    IReadOnlyList<AeronaveOpcionDto> Aeronaves,
    IReadOnlyList<PuertaOpcionDto> Puertas);

public sealed record RutaOpcionDto(int RutaId, string Etiqueta);

public sealed record AeronaveOpcionDto(int AeronaveId, string Etiqueta);

public sealed record PuertaOpcionDto(int PuertaId, string Etiqueta);
