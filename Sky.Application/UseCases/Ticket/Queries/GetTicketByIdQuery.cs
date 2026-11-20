using MediatR;
using Sky.Domain.Ports;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Queries;

public sealed record GetTicketByIdQuery(
    int TicketId) : IRequest<TicketEntity?>;

internal sealed class GetTicketByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketByIdQuery, TicketEntity?>
{
    public Task<TicketEntity?> Handle(GetTicketByIdQuery request, CancellationToken ct)
        => uow.Repository<TicketEntity>().GetByIdAsync(request.TicketId, ct);
}
