using MediatR;
using SkyPort.Application.Dtos.TripulacionVuelo;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.TripulacionVuelo.Queries;
public sealed record GetTripulacionesVueloQuery : IRequest<IReadOnlyList<TripulacionVueloDto>>;
public sealed class GetTripulacionesVueloQueryHandler(IUnitOfWork work) : IRequestHandler<GetTripulacionesVueloQuery, IReadOnlyList<TripulacionVueloDto>>
{
    public async Task<IReadOnlyList<TripulacionVueloDto>> Handle(GetTripulacionesVueloQuery request, CancellationToken cancellationToken)
    {
        var rows = await work.TripulacionesVuelo.GetAllAsync(cancellationToken);
        return rows.OrderBy(x => x.VueloId).ThenBy(x => x.EmpleadoId).Select(x => new TripulacionVueloDto(x.VueloId, x.EmpleadoId, x.RolAbordo)).ToList();
    }
}
