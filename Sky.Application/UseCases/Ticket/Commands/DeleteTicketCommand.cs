using MediatR;
using Sky.Domain.Ports;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Commands;

public sealed record DeleteTicketCommand(
    int TicketId) : IRequest<bool>;

internal sealed class DeleteTicketHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteTicketCommand, bool>
{
    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TicketEntity>().GetByIdAsync(request.TicketId, ct);
        if (entity is null) return false;

        uow.Repository<TicketEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
