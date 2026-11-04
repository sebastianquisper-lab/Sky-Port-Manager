using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Rules.Vuelos;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record CreateVueloCommand(
    string NumeroVuelo,
    int RutaId,
    int AeronaveId,
    int? PuertaId,
    DateTime FechaSalida,
    DateTime FechaLlegada) : IRequest<Result<int>>;

public sealed class CreateVueloCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateVueloCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateVueloCommand request, CancellationToken cancellationToken)
    {
        var input = VueloCreateInput.Create(
            request.NumeroVuelo,
            request.RutaId,
            request.AeronaveId,
            request.PuertaId,
            request.FechaSalida,
            request.FechaLlegada);
        if (!input.IsSuccess || input.Value is null)
        {
            return Result<int>.Failure(input.Error!);
        }

        var value = input.Value;
        if (await unitOfWork.Rutas.GetByIdAsync(value.RutaId, cancellationToken) is null ||
            await unitOfWork.Aeronaves.GetByIdAsync(value.AeronaveId, cancellationToken) is null)
        {
            return Result<int>.Failure(Error.Validation("La ruta o aeronave seleccionada ya no existe."));
        }

        if (value.PuertaId.HasValue &&
            await unitOfWork.Puertas.GetByIdAsync(value.PuertaId.Value, cancellationToken) is null)
        {
            return Result<int>.Failure(Error.Validation("La puerta seleccionada ya no existe."));
        }

        var duplicate = await unitOfWork.Vuelos.FirstOrDefaultAsync(
            flight => flight.NumeroVuelo == value.NumeroVuelo && flight.FechaSalida == value.FechaSalida,
            cancellationToken);
        if (duplicate is not null)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe un vuelo con ese número y fecha de salida."));
        }

        var flight = new Vuelo
        {
            NumeroVuelo = value.NumeroVuelo,
            RutaId = value.RutaId,
            AeronaveId = value.AeronaveId,
            PuertaId = value.PuertaId,
            FechaSalida = value.FechaSalida,
            FechaLlegada = value.FechaLlegada,
            Estado = EstadoVuelo.Programado
        };

        await unitOfWork.Vuelos.AddAsync(flight, cancellationToken);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe un vuelo con ese número y fecha de salida."));
        }
        catch (ReferenceConflictException)
        {
            return Result<int>.Failure(Error.Conflict("Una de las referencias seleccionadas dejó de estar disponible."));
        }

        return Result<int>.Success(flight.VueloId);
    }
}
