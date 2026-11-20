using MediatR;
using Sky.Domain.Ports;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.TicketStatus.Commands;

public sealed record UpdateTicketStatusCommand(
    int TicketStatusId,
    string StatusName,
    string? Description) : IRequest<TicketStatusEntity?>;

internal sealed class UpdateTicketStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTicketStatusCommand, TicketStatusEntity?>
{
    public async Task<TicketStatusEntity?> Handle(UpdateTicketStatusCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<TicketStatusEntity>().GetByIdAsync(request.TicketStatusId, ct);
        if (entity is null) return null;

        entity.StatusName = request.StatusName;
        entity.Description = request.Description;

        uow.Repository<TicketStatusEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
