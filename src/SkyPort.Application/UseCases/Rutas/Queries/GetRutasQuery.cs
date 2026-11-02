using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Rutas.Dtos;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Queries;

public sealed record GetRutasQuery(string? Texto = null) : IRequest<Result<IReadOnlyList<RutaDto>>>;

public sealed class GetRutasQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRutasQuery, Result<IReadOnlyList<RutaDto>>>
{
    public async Task<Result<IReadOnlyList<RutaDto>>> Handle(
        GetRutasQuery request,
        CancellationToken cancellationToken)
    {
        var text = request.Texto?.Trim();
        var routes = string.IsNullOrWhiteSpace(text)
            ? await unitOfWork.Rutas.GetAllAsync(cancellationToken)
            : await unitOfWork.Rutas.FindAsync(
                route => route.CodigoOrigen.Contains(text) ||
                         route.CiudadOrigen.Contains(text) ||
                         route.CodigoDestino.Contains(text) ||
                         route.CiudadDestino.Contains(text),
                cancellationToken);

        var result = routes
            .OrderBy(route => route.CodigoOrigen)
            .ThenBy(route => route.CodigoDestino)
            .Select(route => new RutaDto(
                route.RutaId,
                route.CodigoOrigen,
                route.CiudadOrigen,
                route.CodigoDestino,
                route.CiudadDestino,
                route.DuracionMinutos))
            .ToArray();

        return Result<IReadOnlyList<RutaDto>>.Success(result);
    }
}
