using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Puertas.Commands;

public sealed record CreatePuertaCommand(int TerminalId, string Codigo, bool? Disponible) : IRequest<Result<int>>;

public sealed class CreatePuertaCommandHandler(IUnitOfWork work)
    : IRequestHandler<CreatePuertaCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePuertaCommand request, CancellationToken cancellationToken)
    {
        var codigo = request.Codigo?.Trim() ?? string.Empty;
        if (request.TerminalId <= 0 || codigo.Length is < 1 or > 10 || request.Disponible is null)
            return Result<int>.Failure(Error.Validation("Terminal, código y disponibilidad son obligatorios; el código admite hasta 10 caracteres."));

        if (await work.Terminales.GetByIdAsync(request.TerminalId, cancellationToken) is null)
            return Result<int>.Failure(Error.Validation("La terminal seleccionada no existe."));

        if (await work.Puertas.FirstOrDefaultAsync(
                x => x.TerminalId == request.TerminalId && x.Codigo == codigo,
                cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("Ya existe una puerta con ese código en la terminal seleccionada."));

        var puerta = new Puerta { TerminalId = request.TerminalId, Codigo = codigo, Disponible = request.Disponible.Value };
        await work.Puertas.AddAsync(puerta, cancellationToken);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una puerta con ese código en la terminal seleccionada."));
        }
        catch (ReferenceConflictException)
        {
            return Result<int>.Failure(Error.Conflict("La terminal seleccionada dejó de estar disponible."));
        }

        return Result<int>.Success(puerta.PuertaId);
    }
}
