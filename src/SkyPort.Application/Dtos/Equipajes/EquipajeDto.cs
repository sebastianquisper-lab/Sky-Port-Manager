namespace SkyPort.Application.Dtos.Equipajes;

public sealed record EquipajeDto(int EquipajeId, int BoletoId, string CodigoBoleto, string Codigo, decimal PesoKg, string Estado);
