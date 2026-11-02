using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Vuelos.Rules;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Commands;

public sealed record ChangeVueloEstadoCommand(int VueloId, string NuevoEstado) : IRequest<Result>;

public sealed class ChangeVueloEstadoCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeVueloEstadoCommand, Result>
{
    public async Task<Result> Handle(ChangeVueloEstadoCommand request, CancellationToken cancellationToken)
    {
        var flight = await unitOfWork.Vuelos.GetByIdAsync(request.VueloId, cancellationToken);
        if (flight is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        if (!TransicionesEstadoVuelo.EsValida(flight.Estado, request.NuevoEstado))
        {
            return Result.Failure(Error.Conflict(
                $"No se permite cambiar el estado de {flight.Estado} a {request.NuevoEstado}."));
        }

        if (request.NuevoEstado == EstadoVuelo.Embarcando && !flight.PuertaId.HasValue)
        {
            return Result.Failure(Error.Conflict("Asigna una puerta antes de cambiar el vuelo a Embarcando."));
        }

        flight.Estado = request.NuevoEstado;
        unitOfWork.Vuelos.Update(flight);

        if (request.NuevoEstado == EstadoVuelo.Cancelado)
        {
            await CancelReservationsAndTicketsAsync(flight.VueloId, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task CancelReservationsAndTicketsAsync(int vueloId, CancellationToken cancellationToken)
    {
        var reservations = await unitOfWork.Reservas.FindAsync(
            reservation => reservation.VueloId == vueloId,
            cancellationToken);

        foreach (var reservation in reservations.Where(item => item.Estado == EstadoReserva.Confirmada))
        {
            reservation.Estado = EstadoReserva.Cancelada;
            unitOfWork.Reservas.Update(reservation);
        }

        var reservationIds = reservations.Select(reservation => reservation.ReservaId).ToArray();
        if (reservationIds.Length == 0)
        {
            return;
        }

        var tickets = await unitOfWork.Boletos.FindAsync(
            ticket => reservationIds.Contains(ticket.ReservaId),
            cancellationToken);
        foreach (var ticket in tickets.Where(item => item.Estado != EstadoBoleto.Abordado))
        {
            ticket.Estado = EstadoBoleto.Cancelado;
            unitOfWork.Boletos.Update(ticket);
        }
    }
}
