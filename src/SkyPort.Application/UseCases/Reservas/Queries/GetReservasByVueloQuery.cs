using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetReservasByVueloQuery(int VueloId) : IRequest<IReadOnlyList<ReservaDto>>;

public sealed class GetReservasByVueloQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasByVueloQuery, IReadOnlyList<ReservaDto>>
{
    public Task<IReadOnlyList<ReservaDto>> Handle(
        GetReservasByVueloQuery request,
        CancellationToken cancellationToken) => queries.GetByVueloAsync(request.VueloId, cancellationToken);
}
