using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;

namespace SkyPort.Application.UseCases.Pasajeros;

public sealed record UpdatePasajeroCommand(
    int PasajeroId,
    string TipoDocumento,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    DateOnly FechaNacimiento,
    string Nacionalidad,
    string? Email,
    string? Telefono) : IRequest<Result<PasajeroDto>>;

public sealed record DeletePasajeroCommand(int PasajeroId) : IRequest<Result>;
