using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Terminales.Commands;

public sealed record CreateTerminalCommand(string Nombre) : IRequest<Result<int>>;

public sealed class CreateTerminalCommandHandler(IUnitOfWork work)
    : IRequestHandler<CreateTerminalCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTerminalCommand request, CancellationToken cancellationToken)
    {
        var nombre = request.Nombre?.Trim() ?? string.Empty;
        if (nombre.Length is < 1 or > 50)
            return Result<int>.Failure(Error.Validation("El nombre es obligatorio y admite hasta 50 caracteres."));

        if (await work.Terminales.FirstOrDefaultAsync(x => x.Nombre == nombre, cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("Ya existe una terminal con ese nombre."));

        var terminal = new Terminal { Nombre = nombre };
        await work.Terminales.AddAsync(terminal, cancellationToken);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una terminal con ese nombre."));
        }

        return Result<int>.Success(terminal.TerminalId);
    }
}
