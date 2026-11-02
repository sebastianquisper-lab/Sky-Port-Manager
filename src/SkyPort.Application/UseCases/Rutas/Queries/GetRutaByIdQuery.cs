using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Rutas.Dtos;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Queries;

public sealed record GetRutaByIdQuery(int RutaId) : IRequest<Result<RutaDto>>;

public sealed class GetRutaByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRutaByIdQuery, Result<RutaDto>>
{
    public async Task<Result<RutaDto>> Handle(GetRutaByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await unitOfWork.Rutas.GetByIdAsync(request.RutaId, cancellationToken);
        if (route is null)
        {
            return Result<RutaDto>.Failure(Error.NotFound("No se encontró la ruta."));
        }

        return Result<RutaDto>.Success(new RutaDto(
            route.RutaId,
            route.CodigoOrigen,
            route.CiudadOrigen,
            route.CodigoDestino,
            route.CiudadDestino,
            route.DuracionMinutos));
    }
}
