using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.Aeronaves.Commands;
public sealed record DeleteAeronaveCommand(int AeronaveId) : IRequest<Result>;
public sealed class DeleteAeronaveCommandHandler(IUnitOfWork work) : IRequestHandler<DeleteAeronaveCommand, Result>
{
    public async Task<Result> Handle(DeleteAeronaveCommand request, CancellationToken cancellationToken)
    {
        var row = await work.Aeronaves.GetByIdAsync(request.AeronaveId, cancellationToken);
        if (row is null) return Result.Failure(Error.NotFound("Aeronave no encontrada."));
        if (await work.Vuelos.CountAsync(x => x.AeronaveId == row.AeronaveId, cancellationToken) > 0)
            return Result.Failure(Error.Conflict("No se puede eliminar una aeronave con vuelos."));
        work.Aeronaves.Remove(row);
        try { await work.CompleteAsync(cancellationToken); }
        catch (ReferenceConflictException) { return Result.Failure(Error.Conflict("La aeronave tiene vuelos asociados.")); }
        return Result.Success();
    }
}
