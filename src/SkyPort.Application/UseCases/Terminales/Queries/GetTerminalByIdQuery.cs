using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Terminales;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Terminales.Queries;

public sealed record GetTerminalByIdQuery(int TerminalId) : IRequest<Result<TerminalDto>>;

public sealed class GetTerminalByIdQueryHandler(IUnitOfWork work)
    : IRequestHandler<GetTerminalByIdQuery, Result<TerminalDto>>
{
    public async Task<Result<TerminalDto>> Handle(
        GetTerminalByIdQuery request,
        CancellationToken cancellationToken)
    {
        var terminal = await work.Terminales.GetByIdAsync(request.TerminalId, cancellationToken);
        return terminal is null
            ? Result<TerminalDto>.Failure(Error.NotFound("Terminal no encontrada."))
            : Result<TerminalDto>.Success(new TerminalDto(terminal.TerminalId, terminal.Nombre));
    }
}
