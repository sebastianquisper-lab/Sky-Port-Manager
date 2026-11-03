using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Reservas.Dtos;

namespace SkyPort.Application.UseCases.Reservas;

public sealed record CreateReservaCommand(int PasajeroId, int VueloId, string Asiento)
    : IRequest<Result<ReservaDto>>;

public sealed record CancelReservaCommand(int ReservaId) : IRequest<Result>;
