using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Embarques.Commands;

public sealed record CreateEmbarqueCommand(int BoletoId) : IRequest<Result<int>>;

public sealed class CreateEmbarqueCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmbarqueCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateEmbarqueCommand request, CancellationToken cancellationToken)
    {
        if (request.BoletoId <= 0)
        {
            return Result<int>.Failure(Error.Validation("Selecciona un boleto válido."));
        }

        var ticket = await unitOfWork.Boletos.GetByIdAsync(request.BoletoId, cancellationToken);
        if (ticket is null)
        {
            return Result<int>.Failure(Error.NotFound("No se encontró el boleto."));
        }

        if (ticket.Estado != EstadoBoleto.CheckInRealizado)
        {
            return Result<int>.Failure(Error.Conflict("Solo se puede embarcar un boleto con check-in realizado."));
        }

        if (await unitOfWork.CheckIns.FirstOrDefaultAsync(
                checkIn => checkIn.BoletoId == ticket.BoletoId,
                cancellationToken) is null)
        {
            return Result<int>.Failure(Error.Conflict("El boleto no tiene un registro de check-in."));
        }

        var reservation = await unitOfWork.Reservas.GetByIdAsync(ticket.ReservaId, cancellationToken);
        if (reservation is null)
        {
            return Result<int>.Failure(Error.NotFound("No se encontró la reserva del boleto."));
        }

        if (reservation.Estado != EstadoReserva.Confirmada)
        {
            return Result<int>.Failure(Error.Conflict("No se puede embarcar un boleto de una reserva no confirmada."));
        }

        var flight = await unitOfWork.Vuelos.GetByIdAsync(reservation.VueloId, cancellationToken);
        if (flight is null)
        {
            return Result<int>.Failure(Error.NotFound("No se encontró el vuelo de la reserva."));
        }

        if (await unitOfWork.Embarques.FirstOrDefaultAsync(
                boarding => boarding.BoletoId == ticket.BoletoId,
                cancellationToken) is not null)
        {
            return Result<int>.Failure(Error.Conflict("El boleto ya tiene un embarque registrado."));
        }

        if (flight.PuertaId is int gateId && await unitOfWork.Puertas.GetByIdAsync(gateId, cancellationToken) is null)
        {
            return Result<int>.Failure(Error.Conflict("La puerta asignada al vuelo ya no está disponible."));
        }

        var boarding = new Embarque
        {
            BoletoId = ticket.BoletoId,
            PuertaId = flight.PuertaId,
            FechaHora = DateTime.Now
        };
        ticket.Estado = EstadoBoleto.Abordado;
        await unitOfWork.Embarques.AddAsync(boarding, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("El boleto ya tiene un embarque registrado."));
        }
        catch (ReferenceConflictException)
        {
            return Result<int>.Failure(Error.Conflict("Una referencia del vuelo o la puerta dejó de estar disponible."));
        }

        return Result<int>.Success(boarding.EmbarqueId);
    }
}
