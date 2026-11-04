using MediatR;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVueloCreateOptionsQuery : IRequest<VueloLookupsDto>;

public sealed class GetVueloCreateOptionsQueryHandler(IVueloQueries queries)
    : IRequestHandler<GetVueloCreateOptionsQuery, VueloLookupsDto>
{
    public Task<VueloLookupsDto> Handle(GetVueloCreateOptionsQuery request, CancellationToken cancellationToken) =>
        queries.GetCreateOptionsAsync(cancellationToken);
}
