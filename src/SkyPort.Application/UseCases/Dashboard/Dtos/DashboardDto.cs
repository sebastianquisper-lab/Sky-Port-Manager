namespace SkyPort.Application.UseCases.Dashboard.Dtos;

public sealed record DashboardDto(
    int VuelosHoy,
    IReadOnlyList<VuelosPorEstadoDto> VuelosPorEstado,
    IReadOnlyList<ProximoVueloDto> ProximosVuelos,
    int ReservasConfirmadasHoy,
    int PasajerosAbordadosHoy);

public sealed record VuelosPorEstadoDto(string Estado, int Cantidad);

public sealed record ProximoVueloDto(
    string NumeroVuelo,
    string CodigoOrigen,
    string CodigoDestino,
    DateTime FechaSalida,
    string Estado);
