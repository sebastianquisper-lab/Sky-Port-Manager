using System.Security.Cryptography;
using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Boletos.Commands;

public sealed record CreateBoletoCommand(int ReservaId) : IRequest<Result<int>>;

public sealed class CreateBoletoCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateBoletoCommand, Result<int>>
{
    private const int MaxAttempts = 5;
    private const string CodeIndex = "UQ_Boleto_CodigoBoleto";
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public async Task<Result<int>> Handle(CreateBoletoCommand request, CancellationToken cancellationToken)
    {
        var reserva = await unitOfWork.Reservas.GetByIdAsync(request.ReservaId, cancellationToken);
        if (reserva is null) return Result<int>.Failure(Error.NotFound("No se encontró la reserva."));
        if (reserva.Estado != EstadoReserva.Confirmada) return Result<int>.Failure(Error.Conflict("Solo se emiten boletos para reservas confirmadas."));
        if (await unitOfWork.Boletos.FirstOrDefaultAsync(b => b.ReservaId == reserva.ReservaId, cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("La reserva ya tiene un boleto emitido."));

        var boleto = new Boleto { ReservaId = reserva.ReservaId, FechaEmision = DateTime.Now, Estado = EstadoBoleto.Emitido };
        await unitOfWork.Boletos.AddAsync(boleto, cancellationToken);
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            boleto.CodigoBoleto = GenerateCode();
            try
            {
                await unitOfWork.CompleteAsync(cancellationToken);
                return Result<int>.Success(boleto.BoletoId);
            }
            catch (DuplicateKeyException exception) when (exception.Message.Contains(CodeIndex, StringComparison.OrdinalIgnoreCase))
            {
                if (attempt == MaxAttempts - 1) return Result<int>.Failure(Error.Conflict("No se pudo generar un código único para el boleto."));
            }
            catch (DuplicateKeyException)
            {
                return Result<int>.Failure(Error.Conflict("La reserva ya tiene un boleto o el código ya está en uso."));
            }
        }
        return Result<int>.Failure(Error.Conflict("No se pudo emitir el boleto."));
    }

    private static string GenerateCode()
    {
        Span<char> code = stackalloc char[14];
        for (var i = 0; i < code.Length; i++) code[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        return new string(code);
    }
}
