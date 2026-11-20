using MediatR;
using Sky.Domain.Ports;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.TicketStatus.Queries;

public sealed record GetTicketStatusesQuery : IRequest<IReadOnlyList<TicketStatusEntity>>;

internal sealed class GetTicketStatusesHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketStatusesQuery, IReadOnlyList<TicketStatusEntity>>
{
    public Task<IReadOnlyList<TicketStatusEntity>> Handle(GetTicketStatusesQuery request, CancellationToken ct)
        => uow.Repository<TicketStatusEntity>().GetAllAsync(ct);
}
