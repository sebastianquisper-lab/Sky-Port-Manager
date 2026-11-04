using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Reservas.Dtos;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetReservasQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<ReservaDto>>;

public sealed class GetReservasQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasQuery, PagedResult<ReservaDto>>
{
    public Task<PagedResult<ReservaDto>> Handle(GetReservasQuery request, CancellationToken cancellationToken) =>
        queries.GetPageAsync(Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), cancellationToken);
}
