using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Pasajeros.Queries;

public sealed record GetPasajeroByIdQuery(int PasajeroId) : IRequest<Result<PasajeroDto>>;

public sealed class GetPasajeroByIdQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajeroByIdQuery, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(GetPasajeroByIdQuery request, CancellationToken cancellationToken)
    {
        var pasajero = await queries.GetByIdAsync(request.PasajeroId, cancellationToken);
        return pasajero is null
            ? Result<PasajeroDto>.Failure(Error.NotFound("No se encontró el pasajero."))
            : Result<PasajeroDto>.Success(pasajero);
    }
}
