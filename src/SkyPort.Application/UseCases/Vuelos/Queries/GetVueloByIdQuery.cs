using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVueloByIdQuery(int VueloId) : IRequest<Result<VueloDetailDto>>;

public sealed class GetVueloByIdQueryHandler(IVueloQueries vueloQueries)
    : IRequestHandler<GetVueloByIdQuery, Result<VueloDetailDto>>
{
    public async Task<Result<VueloDetailDto>> Handle(
        GetVueloByIdQuery request,
        CancellationToken cancellationToken)
    {
        var flight = await vueloQueries.GetDetailAsync(request.VueloId, cancellationToken);
        return flight is null
            ? Result<VueloDetailDto>.Failure(Error.NotFound("No se encontró el vuelo."))
            : Result<VueloDetailDto>.Success(flight);
    }
}
