namespace SkyPort.Application.Dtos.Aeronaves;
public sealed record AeronaveDto(int AeronaveId, int AerolineaId, string Matricula, string Modelo, int Capacidad, string Estado);
