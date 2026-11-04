using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Puertas.Commands;

public sealed record DeletePuertaCommand(int PuertaId) : IRequest<Result>;

public sealed class DeletePuertaCommandHandler(IUnitOfWork work)
    : IRequestHandler<DeletePuertaCommand, Result>
{
    public async Task<Result> Handle(DeletePuertaCommand request, CancellationToken cancellationToken)
    {
        var gate = await work.Puertas.GetByIdAsync(request.PuertaId, cancellationToken);
        if (gate is null) return Result.Failure(Error.NotFound("Puerta no encontrada."));

        var hasFlights = await work.Vuelos.CountAsync(x => x.PuertaId == gate.PuertaId, cancellationToken) > 0;
        var hasBoardings = await work.Embarques.CountAsync(x => x.PuertaId == gate.PuertaId, cancellationToken) > 0;
        if (hasFlights || hasBoardings)
            return Result.Failure(Error.Conflict("No se puede eliminar una puerta con vuelos o embarques asociados."));

        work.Puertas.Remove(gate);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar una puerta con vuelos o embarques asociados."));
        }

        return Result.Success();
    }
}
