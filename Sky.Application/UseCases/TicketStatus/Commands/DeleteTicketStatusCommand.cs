using MediatR;
using Sky.Domain.Ports;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.TicketStatus.Commands;

public sealed record DeleteTicketStatusCommand(
    int TicketStatusId) : IRequest<bool>;

internal sealed class DeleteTicketStatusHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteTicketStatusCommand, bool>
{
    public async Task<bool> Handle(DeleteTicketStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TicketStatusEntity>().GetByIdAsync(request.TicketStatusId, ct);
        if (entity is null) return false;

        uow.Repository<TicketStatusEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
