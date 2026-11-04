namespace SkyPort.Application.Dtos.Embarques;

public sealed record EmbarqueCreateOptionsDto(IReadOnlyList<EmbarqueBoletoOptionDto> Boletos);

public sealed record EmbarqueBoletoOptionDto(
    int BoletoId,
    string Etiqueta,
    int? PuertaId,
    string? Puerta);
