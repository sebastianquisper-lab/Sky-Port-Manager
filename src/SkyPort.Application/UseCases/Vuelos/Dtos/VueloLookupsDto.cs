namespace SkyPort.Application.UseCases.Vuelos.Dtos;

public sealed record RutaLookupDto(
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino);

public sealed record AerolineaLookupDto(int AerolineaId, string Nombre);

public sealed record AeronaveLookupDto(
    int AeronaveId,
    string Matricula,
    string Modelo,
    string Aerolinea,
    int Capacidad);

public sealed record PuertaLookupDto(int PuertaId, string Terminal, string Codigo);

public sealed record VueloLookupsDto(
    IReadOnlyList<RutaLookupDto> Rutas,
    IReadOnlyList<AerolineaLookupDto> Aerolineas,
    IReadOnlyList<AeronaveLookupDto> Aeronaves,
    IReadOnlyList<PuertaLookupDto> Puertas);
