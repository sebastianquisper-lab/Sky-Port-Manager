using MediatR;
using Sky.Domain.Ports;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.TicketStatus.Commands;

public sealed record CreateTicketStatusCommand(
    string StatusName,
    string? Description) : IRequest<TicketStatusEntity>;

internal sealed class CreateTicketStatusHandler(IUnitOfWork uow)
    : IRequestHandler<CreateTicketStatusCommand, TicketStatusEntity>
{
    public async Task<TicketStatusEntity> Handle(CreateTicketStatusCommand request, CancellationToken ct)
    {
        var entity = new TicketStatusEntity
        {
            StatusName = request.StatusName,
            Description = request.Description
        };

        await uow.Repository<TicketStatusEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
