using System.Security.Cryptography;
using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Reservas;

public sealed class CreateReservaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateReservaCommand, Result<ReservaDto>>
{
    private const int MaxCodeAttempts = 5;
    private const string CodeAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
    private const string ReservationCodeIndex = "UQ_Reserva_Codigo";
    private const string SeatTakenMessage = "El asiento ya fue tomado.";

    public async Task<Result<ReservaDto>> Handle(
        CreateReservaCommand request,
        CancellationToken cancellationToken)
    {
        var pasajero = await unitOfWork.Pasajeros.GetByIdAsync(request.PasajeroId, cancellationToken);
        if (pasajero is null)
        {
            return Result<ReservaDto>.Failure(Error.NotFound("No se encontró el pasajero."));
        }

        var vuelo = await unitOfWork.Vuelos.GetByIdAsync(request.VueloId, cancellationToken);
        if (vuelo is null)
        {
            return Result<ReservaDto>.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        if (vuelo.Estado is not (EstadoVuelo.Programado or EstadoVuelo.Retrasado))
        {
            return Result<ReservaDto>.Failure(Error.Conflict("Solo se puede reservar en vuelos programados o retrasados."));
        }

        var aeronave = await unitOfWork.Aeronaves.GetByIdAsync(vuelo.AeronaveId, cancellationToken);
        if (aeronave is null)
        {
            return Result<ReservaDto>.Failure(Error.NotFound("No se encontró la aeronave del vuelo."));
        }

        var asiento = request.Asiento?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!AsientoLayout.EsValido(asiento, aeronave.Capacidad))
        {
            return Result<ReservaDto>.Failure(Error.Validation("El asiento indicado no existe en esta aeronave."));
        }

        var reservasConfirmadas = await unitOfWork.Reservas.CountAsync(
            reserva => reserva.VueloId == vuelo.VueloId && reserva.Estado == EstadoReserva.Confirmada,
            cancellationToken);
        if (reservasConfirmadas >= aeronave.Capacidad)
        {
            return Result<ReservaDto>.Failure(Error.Conflict("El vuelo no tiene asientos disponibles."));
        }

        var asientoOcupado = await unitOfWork.Reservas.FirstOrDefaultAsync(
            reserva => reserva.VueloId == vuelo.VueloId &&
                       reserva.Asiento == asiento &&
                       reserva.Estado == EstadoReserva.Confirmada,
            cancellationToken);
        if (asientoOcupado is not null)
        {
            return Result<ReservaDto>.Failure(Error.Conflict(SeatTakenMessage));
        }

        var pasajeroYaReservado = await unitOfWork.Reservas.FirstOrDefaultAsync(
            reserva => reserva.VueloId == vuelo.VueloId &&
                       reserva.PasajeroId == pasajero.PasajeroId &&
                       reserva.Estado == EstadoReserva.Confirmada,
            cancellationToken);
        if (pasajeroYaReservado is not null)
        {
            return Result<ReservaDto>.Failure(Error.Conflict("El pasajero ya tiene una reserva confirmada en este vuelo."));
        }

        var ruta = await unitOfWork.Rutas.GetByIdAsync(vuelo.RutaId, cancellationToken);
        if (ruta is null)
        {
            return Result<ReservaDto>.Failure(Error.NotFound("No se encontró la ruta del vuelo."));
        }

        var reserva = new Reserva
        {
            PasajeroId = pasajero.PasajeroId,
            VueloId = vuelo.VueloId,
            Asiento = asiento,
            Estado = EstadoReserva.Confirmada,
            FechaReserva = DateTime.Now
        };
        await unitOfWork.Reservas.AddAsync(reserva, cancellationToken);

        for (var attempt = 0; attempt < MaxCodeAttempts; attempt++)
        {
            reserva.Codigo = GenerateCode();
            try
            {
                await unitOfWork.CompleteAsync(cancellationToken);
                return Result<ReservaDto>.Success(new ReservaDto(
                    reserva.ReservaId,
                    reserva.Codigo,
                    pasajero.PasajeroId,
                    $"{pasajero.Nombres} {pasajero.Apellidos}",
                    pasajero.TipoDocumento,
                    pasajero.NumeroDocumento,
                    vuelo.VueloId,
                    vuelo.NumeroVuelo,
                    $"{ruta.CodigoOrigen} - {ruta.CodigoDestino}",
                    vuelo.FechaSalida,
                    reserva.Asiento,
                    reserva.FechaReserva,
                    reserva.Estado,
                    null,
                    null));
            }
            catch (DuplicateKeyException exception) when (IsCodeCollision(exception))
            {
                if (attempt == MaxCodeAttempts - 1)
                {
                    return Result<ReservaDto>.Failure(Error.Conflict("No se pudo generar un código único para la reserva."));
                }
            }
            catch (DuplicateKeyException)
            {
                return Result<ReservaDto>.Failure(Error.Conflict(SeatTakenMessage));
            }
        }

        return Result<ReservaDto>.Failure(Error.Conflict("No se pudo crear la reserva."));
    }

    private static string GenerateCode()
    {
        Span<char> chars = stackalloc char[6];
        for (var index = 0; index < chars.Length; index++)
        {
            chars[index] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
        }

        return new string(chars);
    }

    private static bool IsCodeCollision(DuplicateKeyException exception) =>
        exception.Message.Contains(ReservationCodeIndex, StringComparison.OrdinalIgnoreCase);
}

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

        var boleto = await unitOfWork.Boletos.FirstOrDefaultAsync(
            item => item.ReservaId == reserva.ReservaId,
            cancellationToken);
        if (boleto is not null && boleto.Estado != EstadoBoleto.Emitido)
        {
            return Result.Failure(Error.Conflict("No se puede cancelar la reserva después del check-in."));
        }

        reserva.Estado = EstadoReserva.Cancelada;
        unitOfWork.Reservas.Update(reserva);
        if (boleto is not null)
        {
            boleto.Estado = EstadoBoleto.Cancelado;
            unitOfWork.Boletos.Update(boleto);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
