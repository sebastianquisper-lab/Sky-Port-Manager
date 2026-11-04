using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Rutas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Rutas.Queries;

public sealed record GetRutaByIdQuery(int RutaId) : IRequest<Result<RutaDto>>;

public sealed class GetRutaByIdQueryHandler(IRutaQueries queries)
    : IRequestHandler<GetRutaByIdQuery, Result<RutaDto>>
{
    public async Task<Result<RutaDto>> Handle(GetRutaByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.RutaId <= 0)
        {
            return Result<RutaDto>.Failure(Error.NotFound("No se encontró la ruta."));
        }

        var route = await queries.GetByIdAsync(request.RutaId, cancellationToken);
        return route is null
            ? Result<RutaDto>.Failure(Error.NotFound("No se encontró la ruta."))
            : Result<RutaDto>.Success(route);
    }
}
