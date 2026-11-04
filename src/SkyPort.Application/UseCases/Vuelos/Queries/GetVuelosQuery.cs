using MediatR;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVuelosQuery : IRequest<IReadOnlyList<VueloListItemDto>>;

public sealed class GetVuelosQueryHandler(IVueloQueries queries)
    : IRequestHandler<GetVuelosQuery, IReadOnlyList<VueloListItemDto>>
{
    public Task<IReadOnlyList<VueloListItemDto>> Handle(GetVuelosQuery request, CancellationToken cancellationToken) =>
        queries.GetAllAsync(cancellationToken);
}
