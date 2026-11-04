using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Pasajeros.Queries;

public sealed record GetPasajerosQuery(string? Busqueda = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<PasajeroDto>>;

public sealed class GetPasajerosQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajerosQuery, PagedResult<PasajeroDto>>
{
    public Task<PagedResult<PasajeroDto>> Handle(GetPasajerosQuery request, CancellationToken cancellationToken) =>
        queries.GetPageAsync(request.Busqueda, Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), cancellationToken);
}
