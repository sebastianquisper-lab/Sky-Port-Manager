using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVuelosQuery(VueloFilter Filter, int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedResult<VueloListItemDto>>>;

public sealed class GetVuelosQueryHandler(IVueloQueries vueloQueries)
    : IRequestHandler<GetVuelosQuery, Result<PagedResult<VueloListItemDto>>>
{
    public async Task<Result<PagedResult<VueloListItemDto>>> Handle(
        GetVuelosQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Filter.Desde.HasValue && request.Filter.Hasta.HasValue &&
            request.Filter.Desde.Value > request.Filter.Hasta.Value)
        {
            return Result<PagedResult<VueloListItemDto>>.Failure(
                Error.Validation("La fecha desde no puede ser posterior a la fecha hasta."));
        }

        if (request.Page < 1 || request.PageSize is < 1 or > 100)
        {
            return Result<PagedResult<VueloListItemDto>>.Failure(
                Error.Validation("La página debe ser positiva y el tamaño debe estar entre 1 y 100."));
        }

        var result = await vueloQueries.SearchAsync(
            request.Filter,
            request.Page,
            request.PageSize,
            cancellationToken);
        return Result<PagedResult<VueloListItemDto>>.Success(result);
    }
}
