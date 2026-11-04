using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Puertas.Commands;

public sealed record UpdatePuertaCommand(int PuertaId, int TerminalId, string Codigo, bool? Disponible) : IRequest<Result>;

public sealed class UpdatePuertaCommandHandler(IUnitOfWork work)
    : IRequestHandler<UpdatePuertaCommand, Result>
{
    public async Task<Result> Handle(UpdatePuertaCommand request, CancellationToken cancellationToken)
    {
        var gate = await work.Puertas.GetByIdAsync(request.PuertaId, cancellationToken);
        if (gate is null) return Result.Failure(Error.NotFound("Puerta no encontrada."));

        var code = request.Codigo?.Trim() ?? string.Empty;
        if (request.TerminalId <= 0 || code.Length is < 1 or > 10 || request.Disponible is null)
            return Result.Failure(Error.Validation("Terminal, código y disponibilidad son obligatorios; el código admite hasta 10 caracteres."));

        if (await work.Terminales.GetByIdAsync(request.TerminalId, cancellationToken) is null)
            return Result.Failure(Error.Validation("La terminal seleccionada no existe."));

        var assignmentChanged = gate.TerminalId != request.TerminalId || gate.Codigo != code;
        if (assignmentChanged)
        {
            var hasFlights = await work.Vuelos.CountAsync(x => x.PuertaId == gate.PuertaId, cancellationToken) > 0;
            var hasBoardings = await work.Embarques.CountAsync(x => x.PuertaId == gate.PuertaId, cancellationToken) > 0;
            if (hasFlights || hasBoardings)
                return Result.Failure(Error.Conflict("No se puede cambiar la terminal o el código de una puerta con historial asociado."));
        }

        if (await work.Puertas.FirstOrDefaultAsync(
                x => x.PuertaId != gate.PuertaId && x.TerminalId == request.TerminalId && x.Codigo == code,
                cancellationToken) is not null)
            return Result.Failure(Error.Conflict("Ya existe una puerta con ese código en la terminal seleccionada."));

        gate.TerminalId = request.TerminalId;
        gate.Codigo = code;
        gate.Disponible = request.Disponible.Value;
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("Ya existe una puerta con ese código en la terminal seleccionada."));
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("La terminal seleccionada dejó de estar disponible."));
        }

        return Result.Success();
    }
}
