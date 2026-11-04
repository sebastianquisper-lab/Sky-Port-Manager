using MediatR;
using SkyPort.Application.Dtos.Rutas;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Queries;

public sealed record GetRutasQuery : IRequest<IReadOnlyList<RutaDto>>;

public sealed class GetRutasQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRutasQuery, IReadOnlyList<RutaDto>>
{
    public async Task<IReadOnlyList<RutaDto>> Handle(GetRutasQuery request, CancellationToken cancellationToken)
    {
        var routes = await unitOfWork.Rutas.GetAllAsync(cancellationToken);
        return routes
            .OrderBy(route => route.CodigoOrigen)
            .ThenBy(route => route.CodigoDestino)
            .ThenBy(route => route.RutaId)
            .Select(route => new RutaDto(
                route.RutaId,
                route.CodigoOrigen,
                route.CiudadOrigen,
                route.CodigoDestino,
                route.CiudadDestino,
                route.DuracionMinutos))
            .ToArray();
    }
}
