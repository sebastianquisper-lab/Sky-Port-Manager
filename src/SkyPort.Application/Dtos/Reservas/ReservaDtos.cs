namespace SkyPort.Application.Dtos.Reservas;

public sealed record VueloAbiertoDto(
    int VueloId,
    string NumeroVuelo,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    DateTime FechaSalida,
    string Estado,
    int Capacidad,
    int ReservasConfirmadas,
    int AsientosLibres);

public sealed record ReservaDto(
    int ReservaId,
    string Codigo,
    int PasajeroId,
    string NombrePasajero,
    string TipoDocumento,
    string NumeroDocumento,
    int VueloId,
    string NumeroVuelo,
    string Ruta,
    DateTime FechaSalida,
    string Asiento,
    DateTime FechaReserva,
    string Estado,
    int? BoletoId,
    string? EstadoBoleto);

public sealed record AsientosVueloDto(
    int Capacidad,
    int Filas,
    IReadOnlyList<string> AsientosOcupados);

public sealed record ReservaBoletoOptionDto(int ReservaId, string Codigo, string NombrePasajero, string NumeroVuelo, string Asiento);
