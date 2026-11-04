namespace SkyPort.Application.Dtos.CheckIn;

public sealed record CheckInDto(int CheckInId, int BoletoId, string CodigoBoleto, int ReservaId,
    string CodigoReserva, string NombrePasajero, int? EmpleadoId, string? NombreEmpleado,
    DateTime FechaHora, bool DocumentoVerificado);
