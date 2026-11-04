using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Boletos.Queries;

public sealed record GetBoletosQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<BoletoDto>>;
public sealed class GetBoletosQueryHandler(IBoletoQueries queries) : IRequestHandler<GetBoletosQuery, PagedResult<BoletoDto>>
{
    public Task<PagedResult<BoletoDto>> Handle(GetBoletosQuery request, CancellationToken ct) =>
        queries.GetPageAsync(Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), ct);
}
