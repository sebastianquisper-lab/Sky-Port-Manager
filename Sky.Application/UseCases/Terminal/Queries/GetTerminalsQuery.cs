using MediatR;
using Sky.Domain.Ports;
using TerminalEntity = Sky.Domain.Entities.Terminal;

namespace Sky.Application.UseCases.Terminal.Queries;

public sealed record GetTerminalsQuery : IRequest<IReadOnlyList<TerminalEntity>>;

internal sealed class GetTerminalsHandler(IUnitOfWork uow)
    : IRequestHandler<GetTerminalsQuery, IReadOnlyList<TerminalEntity>>
{
    public Task<IReadOnlyList<TerminalEntity>> Handle(GetTerminalsQuery request, CancellationToken ct)
        => uow.Repository<TerminalEntity>().GetAllAsync(ct);
}
