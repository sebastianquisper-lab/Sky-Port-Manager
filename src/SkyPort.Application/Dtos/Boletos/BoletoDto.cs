namespace SkyPort.Application.Dtos.Boletos;

public sealed record BoletoDto(int BoletoId, int ReservaId, string CodigoBoleto, DateTime FechaEmision, string Estado,
    string CodigoReserva, string NombrePasajero, string NumeroVuelo, string Asiento);
