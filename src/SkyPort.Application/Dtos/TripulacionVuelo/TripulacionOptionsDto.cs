namespace SkyPort.Application.Dtos.TripulacionVuelo;
public sealed record TripulacionOptionDto(int Id, string Label);
public sealed record TripulacionOptionsDto(IReadOnlyList<TripulacionOptionDto> Vuelos, IReadOnlyList<TripulacionOptionDto> Empleados);
