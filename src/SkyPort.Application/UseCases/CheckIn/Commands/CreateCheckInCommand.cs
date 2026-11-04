using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.CheckIn.Commands;

public sealed record CreateCheckInCommand(int BoletoId, int? EmpleadoId, bool DocumentoVerificado) : IRequest<Result<int>>;

public sealed class CreateCheckInCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCheckInCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCheckInCommand request, CancellationToken cancellationToken)
    {
        if (!request.DocumentoVerificado)
            return Result<int>.Failure(Error.Validation("No se puede completar el check-in sin verificar el documento."));

        var boleto = await unitOfWork.Boletos.GetByIdAsync(request.BoletoId, cancellationToken);
        if (boleto is null) return Result<int>.Failure(Error.NotFound("No se encontró el boleto."));
        if (boleto.Estado != EstadoBoleto.Emitido)
            return Result<int>.Failure(Error.Conflict("Solo se puede registrar check-in para boletos emitidos."));

        var reserva = await unitOfWork.Reservas.GetByIdAsync(boleto.ReservaId, cancellationToken);
        if (reserva is null) return Result<int>.Failure(Error.NotFound("No se encontró la reserva del boleto."));
        if (reserva.Estado != EstadoReserva.Confirmada)
            return Result<int>.Failure(Error.Conflict("No se puede registrar check-in para una reserva cancelada."));

        if (await unitOfWork.CheckIns.FirstOrDefaultAsync(c => c.BoletoId == boleto.BoletoId, cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("El boleto ya tiene un registro de check-in."));

        if (request.EmpleadoId is int empleadoId && await unitOfWork.Empleados.GetByIdAsync(empleadoId, cancellationToken) is null)
            return Result<int>.Failure(Error.NotFound("No se encontró el empleado indicado."));

        var checkIn = new Checkin
        {
            BoletoId = boleto.BoletoId,
            EmpleadoId = request.EmpleadoId,
            FechaHora = DateTime.Now,
            DocumentoVerificado = true
        };
        boleto.Estado = EstadoBoleto.CheckInRealizado;
        await unitOfWork.CheckIns.AddAsync(checkIn, cancellationToken);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<int>.Success(checkIn.CheckInId);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("El boleto ya tiene un registro de check-in."));
        }
    }
}
