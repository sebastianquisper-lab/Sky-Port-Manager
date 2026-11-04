using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Terminales.Commands;

public sealed record UpdateTerminalCommand(int TerminalId, string Nombre) : IRequest<Result>;

public sealed class UpdateTerminalCommandHandler(IUnitOfWork work)
    : IRequestHandler<UpdateTerminalCommand, Result>
{
    public async Task<Result> Handle(UpdateTerminalCommand request, CancellationToken cancellationToken)
    {
        var terminal = await work.Terminales.GetByIdAsync(request.TerminalId, cancellationToken);
        if (terminal is null) return Result.Failure(Error.NotFound("Terminal no encontrada."));

        var nombre = request.Nombre?.Trim() ?? string.Empty;
        if (nombre.Length is < 1 or > 50)
            return Result.Failure(Error.Validation("El nombre es obligatorio y admite hasta 50 caracteres."));

        if (await work.Terminales.FirstOrDefaultAsync(
                x => x.TerminalId != terminal.TerminalId && x.Nombre == nombre,
                cancellationToken) is not null)
            return Result.Failure(Error.Conflict("Ya existe una terminal con ese nombre."));

        terminal.Nombre = nombre;
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("Ya existe una terminal con ese nombre."));
        }

        return Result.Success();
    }
}
