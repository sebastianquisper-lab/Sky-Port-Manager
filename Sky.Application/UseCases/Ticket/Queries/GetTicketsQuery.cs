using MediatR;
using Sky.Domain.Ports;
using TicketEntity = Sky.Domain.Entities.Ticket;

namespace Sky.Application.UseCases.Ticket.Queries;

public sealed record GetTicketsQuery : IRequest<IReadOnlyList<TicketEntity>>;

internal sealed class GetTicketsHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketsQuery, IReadOnlyList<TicketEntity>>
{
    public Task<IReadOnlyList<TicketEntity>> Handle(GetTicketsQuery request, CancellationToken ct)
        => uow.Repository<TicketEntity>().GetAllAsync(ct);
}
