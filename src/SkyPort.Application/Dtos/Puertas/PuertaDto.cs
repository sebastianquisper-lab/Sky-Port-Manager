namespace SkyPort.Application.Dtos.Puertas;

public sealed record PuertaDto(int PuertaId, int TerminalId, string TerminalNombre, string Codigo, bool? Disponible);
