using MediatR;
using Sky.Domain.Ports;
using TerminalEntity = Sky.Domain.Entities.Terminal;

namespace Sky.Application.UseCases.Terminal.Queries;

public sealed record GetTerminalByIdQuery(
    int TerminalId) : IRequest<TerminalEntity?>;

internal sealed class GetTerminalByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetTerminalByIdQuery, TerminalEntity?>
{
    public Task<TerminalEntity?> Handle(GetTerminalByIdQuery request, CancellationToken ct)
        => uow.Repository<TerminalEntity>().GetByIdAsync(request.TerminalId, ct);
}
