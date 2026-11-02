using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVueloLookupsQuery : IRequest<Result<VueloLookupsDto>>;

public sealed class GetVueloLookupsQueryHandler(IVueloQueries vueloQueries)
    : IRequestHandler<GetVueloLookupsQuery, Result<VueloLookupsDto>>
{
    public async Task<Result<VueloLookupsDto>> Handle(
        GetVueloLookupsQuery request,
        CancellationToken cancellationToken)
    {
        var lookups = await vueloQueries.GetLookupsAsync(cancellationToken);
        return Result<VueloLookupsDto>.Success(lookups);
    }
}
