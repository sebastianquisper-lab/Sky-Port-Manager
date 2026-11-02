using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record DeleteVueloCommand(int VueloId) : IRequest<Result>;

public sealed class DeleteVueloCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteVueloCommand, Result>
{
    public async Task<Result> Handle(DeleteVueloCommand request, CancellationToken cancellationToken)
    {
        var flight = await unitOfWork.Vuelos.GetByIdAsync(request.VueloId, cancellationToken);
        if (flight is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        if (flight.Estado != EstadoVuelo.Programado)
        {
            return Result.Failure(Error.Conflict("Solo se pueden eliminar vuelos Programados."));
        }

        var reservationCount = await unitOfWork.Reservas.CountAsync(
            reservation => reservation.VueloId == request.VueloId,
            cancellationToken);
        if (reservationCount > 0)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar un vuelo que ya tiene reservas."));
        }

        unitOfWork.Vuelos.Remove(flight);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
