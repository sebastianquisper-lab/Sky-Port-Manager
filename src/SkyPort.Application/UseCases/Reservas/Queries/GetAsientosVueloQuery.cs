using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetAsientosVueloQuery(int VueloId) : IRequest<Result<AsientosVueloDto>>;

public sealed class GetAsientosVueloQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetAsientosVueloQuery, Result<AsientosVueloDto>>
{
    public async Task<Result<AsientosVueloDto>> Handle(
        GetAsientosVueloQuery request,
        CancellationToken cancellationToken)
    {
        var asientos = await queries.GetAsientosAsync(request.VueloId, cancellationToken);
        return asientos is null
            ? Result<AsientosVueloDto>.Failure(Error.NotFound("No se encontró el vuelo."))
            : Result<AsientosVueloDto>.Success(asientos);
    }
}
