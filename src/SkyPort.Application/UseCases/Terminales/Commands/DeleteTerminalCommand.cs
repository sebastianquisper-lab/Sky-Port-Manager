using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Terminales.Commands;

public sealed record DeleteTerminalCommand(int TerminalId) : IRequest<Result>;

public sealed class DeleteTerminalCommandHandler(IUnitOfWork work)
    : IRequestHandler<DeleteTerminalCommand, Result>
{
    public async Task<Result> Handle(DeleteTerminalCommand request, CancellationToken cancellationToken)
    {
        var terminal = await work.Terminales.GetByIdAsync(request.TerminalId, cancellationToken);
        if (terminal is null) return Result.Failure(Error.NotFound("Terminal no encontrada."));

        if (await work.Puertas.CountAsync(x => x.TerminalId == terminal.TerminalId, cancellationToken) > 0)
            return Result.Failure(Error.Conflict("No se puede eliminar una terminal que tiene puertas."));

        work.Terminales.Remove(terminal);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (ReferenceConflictException)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar una terminal que tiene puertas."));
        }

        return Result.Success();
    }
}
