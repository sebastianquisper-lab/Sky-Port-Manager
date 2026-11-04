using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Commands;

public sealed record DeleteRutaCommand(int RutaId) : IRequest<Result>;

public sealed class DeleteRutaCommandHandler(IUnitOfWork unitOfWork, IRutaQueries queries)
    : IRequestHandler<DeleteRutaCommand, Result>
{
    public async Task<Result> Handle(DeleteRutaCommand request, CancellationToken cancellationToken)
    {
        if (request.RutaId <= 0)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        var route = await unitOfWork.Rutas.GetByIdAsync(request.RutaId, cancellationToken);
        if (route is null)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        if (await queries.IsUsedByFlightsAsync(request.RutaId, cancellationToken))
        {
            return Result.Failure(Error.Conflict("No se puede eliminar una ruta que tiene vuelos asociados."));
        }

        unitOfWork.Rutas.Remove(route);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("La ruta tiene registros relacionados y no se puede eliminar."));
        }

        return Result.Success();
    }
}
