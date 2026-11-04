using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Queries;

public sealed record GetVueloByIdQuery(int VueloId) : IRequest<Result<VueloDetailDto>>;

public sealed class GetVueloByIdQueryHandler(IVueloQueries queries)
    : IRequestHandler<GetVueloByIdQuery, Result<VueloDetailDto>>
{
    public async Task<Result<VueloDetailDto>> Handle(GetVueloByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.VueloId <= 0)
        {
            return Result<VueloDetailDto>.Failure(Error.NotFound("No se encontró el vuelo."));
        }

        var flight = await queries.GetByIdAsync(request.VueloId, cancellationToken);
        return flight is null
            ? Result<VueloDetailDto>.Failure(Error.NotFound("No se encontró el vuelo."))
            : Result<VueloDetailDto>.Success(flight);
    }
}
