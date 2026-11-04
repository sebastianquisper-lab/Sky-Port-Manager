using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;

namespace SkyPort.Application.UseCases.Pasajeros;

public sealed record DeletePasajeroCommand(int PasajeroId) : IRequest<Result>;
