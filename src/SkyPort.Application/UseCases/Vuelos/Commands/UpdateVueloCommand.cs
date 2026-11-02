using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record UpdateVueloCommand(
    int VueloId,
    string NumeroVuelo,
    int RutaId,
    int AeronaveId,
    int? PuertaId,
    DateTime FechaSalida,
    DateTime FechaLlegada) : IRequest<Result>;

public sealed class UpdateVueloCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateVueloCommand, Result>
{
    public async Task<Result> Handle(UpdateVueloCommand request, CancellationToken cancellationToken)
    {
        var flight = await unitOfWork.Vuelos.GetByIdAsync(request.VueloId, cancellationToken);
        if (flight is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        if (flight.Estado is not (EstadoVuelo.Programado or EstadoVuelo.Retrasado))
        {
            return Result.Failure(Error.Conflict("Solo se pueden editar vuelos Programados o Retrasados."));
        }

        var inputResult = VueloInput.Create(
            request.NumeroVuelo,
            request.RutaId,
            request.AeronaveId,
            request.PuertaId,
            request.FechaSalida,
            request.FechaLlegada);
        if (!inputResult.IsSuccess)
        {
            return Result.Failure(inputResult.Error!);
        }

        var input = inputResult.Value!;
        var validation = await CreateVueloCommandHandler.ValidateReferencesAsync(
            input,
            unitOfWork,
            cancellationToken);
        if (validation is not null)
        {
            return Result.Failure(validation);
        }

        flight.NumeroVuelo = input.NumeroVuelo;
        flight.RutaId = input.RutaId;
        flight.AeronaveId = input.AeronaveId;
        flight.PuertaId = input.PuertaId;
        flight.FechaSalida = input.FechaSalida;
        flight.FechaLlegada = input.FechaLlegada;
        unitOfWork.Vuelos.Update(flight);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("Ya existe un vuelo con ese número y fecha de salida."));
        }
    }
}
