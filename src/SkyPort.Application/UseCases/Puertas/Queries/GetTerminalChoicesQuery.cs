using MediatR;
using SkyPort.Application.Dtos.Puertas;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Puertas.Queries;

public sealed record GetTerminalChoicesQuery : IRequest<IReadOnlyList<TerminalChoiceDto>>;

public sealed class GetTerminalChoicesQueryHandler(IUnitOfWork work)
    : IRequestHandler<GetTerminalChoicesQuery, IReadOnlyList<TerminalChoiceDto>>
{
    public async Task<IReadOnlyList<TerminalChoiceDto>> Handle(GetTerminalChoicesQuery request, CancellationToken cancellationToken)
    {
        var terminals = await work.Terminales.GetAllAsync(cancellationToken);
        return terminals.OrderBy(x => x.Nombre).ThenBy(x => x.TerminalId)
            .Select(x => new TerminalChoiceDto(x.TerminalId, x.Nombre)).ToList();
    }
}
