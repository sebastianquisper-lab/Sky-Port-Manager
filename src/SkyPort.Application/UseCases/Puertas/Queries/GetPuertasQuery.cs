using MediatR;
using SkyPort.Application.Dtos.Puertas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Puertas.Queries;

public sealed record GetPuertasQuery : IRequest<IReadOnlyList<PuertaDto>>;

public sealed class GetPuertasQueryHandler(IPuertaQueries queries)
    : IRequestHandler<GetPuertasQuery, IReadOnlyList<PuertaDto>>
{
    public Task<IReadOnlyList<PuertaDto>> Handle(GetPuertasQuery request, CancellationToken cancellationToken) =>
        queries.GetAllAsync(cancellationToken);
}
