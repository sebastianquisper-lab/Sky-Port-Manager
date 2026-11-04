using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record DeleteVueloCommand(int VueloId) : IRequest<Result>;

public sealed class DeleteVueloCommandHandler(IUnitOfWork unitOfWork, IVueloQueries queries)
    : IRequestHandler<DeleteVueloCommand, Result>
{
    public async Task<Result> Handle(DeleteVueloCommand request, CancellationToken cancellationToken)
    {
        if (request.VueloId <= 0)
        {
            return Result.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        var flight = await unitOfWork.Vuelos.GetByIdAsync(request.VueloId, cancellationToken);
        if (flight is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        if (await queries.HasReservationsOrCrewAsync(request.VueloId, cancellationToken))
        {
            return Result.Failure(Error.Conflict("No se puede eliminar un vuelo que tiene reservas o tripulación asociada."));
        }

        unitOfWork.Vuelos.Remove(flight);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("El vuelo tiene registros relacionados y no se puede eliminar."));
        }

        return Result.Success();
    }
}
