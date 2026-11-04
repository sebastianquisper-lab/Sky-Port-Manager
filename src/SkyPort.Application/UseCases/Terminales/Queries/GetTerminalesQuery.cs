using MediatR;
using SkyPort.Application.Dtos.Terminales;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Terminales.Queries;

public sealed record GetTerminalesQuery : IRequest<IReadOnlyList<TerminalDto>>;

public sealed class GetTerminalesQueryHandler(IUnitOfWork work)
    : IRequestHandler<GetTerminalesQuery, IReadOnlyList<TerminalDto>>
{
    public async Task<IReadOnlyList<TerminalDto>> Handle(
        GetTerminalesQuery request,
        CancellationToken cancellationToken)
    {
        var terminals = await work.Terminales.GetAllAsync(cancellationToken);
        return terminals
            .OrderBy(terminal => terminal.Nombre)
            .ThenBy(terminal => terminal.TerminalId)
            .Select(terminal => new TerminalDto(terminal.TerminalId, terminal.Nombre))
            .ToList();
    }
}
