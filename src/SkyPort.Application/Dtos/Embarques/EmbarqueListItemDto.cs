namespace SkyPort.Application.Dtos.Embarques;

public sealed record EmbarqueListItemDto(
    int EmbarqueId,
    string CodigoBoleto,
    string Pasajero,
    string NumeroVuelo,
    string? Puerta,
    DateTime FechaHora);
