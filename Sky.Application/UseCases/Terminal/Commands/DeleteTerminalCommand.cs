using MediatR;
using Sky.Domain.Ports;
using TerminalEntity = Sky.Domain.Entities.Terminal;

namespace Sky.Application.UseCases.Terminal.Commands;

public sealed record DeleteTerminalCommand(
    int TerminalId) : IRequest<bool>;

internal sealed class DeleteTerminalHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteTerminalCommand, bool>
{
    public async Task<bool> Handle(DeleteTerminalCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TerminalEntity>().GetByIdAsync(request.TerminalId, ct);
        if (entity is null) return false;

        uow.Repository<TerminalEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
