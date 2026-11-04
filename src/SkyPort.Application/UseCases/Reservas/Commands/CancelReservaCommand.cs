using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Reservas.Commands;

public sealed record CancelReservaCommand(int ReservaId) : IRequest<Result>;

public sealed class CancelReservaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelReservaCommand, Result>
{
    public async Task<Result> Handle(CancelReservaCommand request, CancellationToken cancellationToken)
    {
        var reserva = await unitOfWork.Reservas.GetByIdAsync(request.ReservaId, cancellationToken);
        if (reserva is null)
        {
            return Result.Failure(Error.NotFound("No se encontró la reserva."));
        }

        if (reserva.Estado != EstadoReserva.Confirmada)
        {
            return Result.Failure(Error.Conflict("Solo se pueden cancelar reservas confirmadas."));
        }

        var boletoEncontrado = await unitOfWork.Boletos.FirstOrDefaultAsync(
            item => item.ReservaId == reserva.ReservaId,
            cancellationToken);
        var boleto = boletoEncontrado is null
            ? null
            : await unitOfWork.Boletos.GetByIdAsync(boletoEncontrado.BoletoId, cancellationToken);
        if (boleto is not null && boleto.Estado != EstadoBoleto.Emitido)
        {
            return Result.Failure(Error.Conflict("No se puede cancelar la reserva después del check-in."));
        }

        reserva.Estado = EstadoReserva.Cancelada;
        if (boleto is not null)
        {
            boleto.Estado = EstadoBoleto.Cancelado;
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
