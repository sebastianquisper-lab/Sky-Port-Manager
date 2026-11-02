using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Commands;

public sealed record DeleteRutaCommand(int RutaId) : IRequest<Result>;

public sealed class DeleteRutaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRutaCommand, Result>
{
    public async Task<Result> Handle(DeleteRutaCommand request, CancellationToken cancellationToken)
    {
        var route = await unitOfWork.Rutas.GetByIdAsync(request.RutaId, cancellationToken);
        if (route is null)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        var hasFlights = await unitOfWork.Vuelos.CountAsync(
            flight => flight.RutaId == request.RutaId,
            cancellationToken);
        if (hasFlights > 0)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar una ruta que ya está asignada a uno o más vuelos."));
        }

        unitOfWork.Rutas.Remove(route);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("No se pudo eliminar la ruta por un conflicto con los datos existentes."));
        }
    }
}
