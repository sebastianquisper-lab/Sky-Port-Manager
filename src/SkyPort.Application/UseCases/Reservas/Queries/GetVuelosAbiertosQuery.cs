using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Reservas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetVuelosAbiertosQuery : IRequest<IReadOnlyList<VueloAbiertoDto>>;

public sealed class GetVuelosAbiertosQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetVuelosAbiertosQuery, IReadOnlyList<VueloAbiertoDto>>
{
    public Task<IReadOnlyList<VueloAbiertoDto>> Handle(
        GetVuelosAbiertosQuery request,
        CancellationToken cancellationToken) => queries.GetVuelosAbiertosAsync(cancellationToken);
}
