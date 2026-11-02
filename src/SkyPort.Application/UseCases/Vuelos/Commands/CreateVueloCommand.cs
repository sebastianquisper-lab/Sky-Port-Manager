using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
using SkyPort.Application.UseCases.Vuelos.Rules;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record CreateVueloCommand(
    string NumeroVuelo,
    int RutaId,
    int AeronaveId,
    int? PuertaId,
    DateTime FechaSalida,
    DateTime FechaLlegada) : IRequest<Result<int>>;

public sealed class CreateVueloCommandHandler(IUnitOfWork unitOfWork, VueloSchedulingRules schedulingRules)
    : IRequestHandler<CreateVueloCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateVueloCommand request, CancellationToken cancellationToken)
    {
        var inputResult = VueloInput.Create(
            request.NumeroVuelo,
            request.RutaId,
            request.AeronaveId,
            request.PuertaId,
            request.FechaSalida,
            request.FechaLlegada);
        if (!inputResult.IsSuccess)
        {
            return Result<int>.Failure(inputResult.Error!);
        }

        var input = inputResult.Value!;
        var validation = await ValidateReferencesAsync(input, unitOfWork, cancellationToken);
        if (validation is not null)
        {
            return Result<int>.Failure(validation);
        }

        var schedulingResult = await schedulingRules.ValidateAsync(
            input.AeronaveId,
            input.PuertaId,
            input.FechaSalida,
            input.FechaLlegada,
            vueloIdIgnorado: null,
            esCreacion: true,
            cancellationToken);
        if (!schedulingResult.IsSuccess)
        {
            return Result<int>.Failure(schedulingResult.Error!);
        }

        var flight = new Vuelo
        {
            NumeroVuelo = input.NumeroVuelo,
            RutaId = input.RutaId,
            AeronaveId = input.AeronaveId,
            PuertaId = input.PuertaId,
            FechaSalida = input.FechaSalida,
            FechaLlegada = input.FechaLlegada,
            Estado = EstadoVuelo.Programado
        };

        await unitOfWork.Vuelos.AddAsync(flight, cancellationToken);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(flight.VueloId);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe un vuelo con ese número y fecha de salida."));
        }
    }

    internal static async Task<Error?> ValidateReferencesAsync(
        VueloInput input,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (await unitOfWork.Rutas.GetByIdAsync(input.RutaId, cancellationToken) is null)
        {
            return Error.Validation("La ruta seleccionada no existe.");
        }

        return null;
    }
}
