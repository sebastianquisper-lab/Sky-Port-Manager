using MediatR;
using Sky.Domain.Ports;
using TicketStatusEntity = Sky.Domain.Entities.TicketStatus;

namespace Sky.Application.UseCases.TicketStatus.Queries;

public sealed record GetTicketStatusByIdQuery(
    int TicketStatusId) : IRequest<TicketStatusEntity?>;

internal sealed class GetTicketStatusByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketStatusByIdQuery, TicketStatusEntity?>
{
    public Task<TicketStatusEntity?> Handle(GetTicketStatusByIdQuery request, CancellationToken ct)
        => uow.Repository<TicketStatusEntity>().GetByIdAsync(request.TicketStatusId, ct);
}
