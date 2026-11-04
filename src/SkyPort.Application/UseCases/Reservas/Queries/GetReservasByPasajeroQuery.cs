using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetReservasByPasajeroQuery(int PasajeroId) : IRequest<IReadOnlyList<ReservaDto>>;

public sealed class GetReservasByPasajeroQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasByPasajeroQuery, IReadOnlyList<ReservaDto>>
{
    public Task<IReadOnlyList<ReservaDto>> Handle(
        GetReservasByPasajeroQuery request,
        CancellationToken cancellationToken) => queries.GetByPasajeroAsync(request.PasajeroId, cancellationToken);
}
