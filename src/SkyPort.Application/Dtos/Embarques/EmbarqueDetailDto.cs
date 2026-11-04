namespace SkyPort.Application.Dtos.Embarques;

public sealed record EmbarqueDetailDto(
    int EmbarqueId,
    string CodigoBoleto,
    string Pasajero,
    string NumeroVuelo,
    string? Puerta,
    DateTime FechaHora);
