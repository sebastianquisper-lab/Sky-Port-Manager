using MediatR;
using SkyPort.Application.Dtos.TripulacionVuelo;
using SkyPort.Domain.Ports;
namespace SkyPort.Application.UseCases.TripulacionVuelo.Queries;
public sealed record GetTripulacionOptionsQuery : IRequest<TripulacionOptionsDto>;
public sealed class GetTripulacionOptionsQueryHandler(IUnitOfWork work) : IRequestHandler<GetTripulacionOptionsQuery, TripulacionOptionsDto>
{
    public async Task<TripulacionOptionsDto> Handle(GetTripulacionOptionsQuery request, CancellationToken cancellationToken)
    {
        var flights = await work.Vuelos.GetAllAsync(cancellationToken);
        var employees = await work.Empleados.GetAllAsync(cancellationToken);
        return new(
            flights.OrderBy(x => x.FechaSalida).Select(x => new TripulacionOptionDto(x.VueloId, $"{x.NumeroVuelo} - {x.FechaSalida:dd/MM/yyyy HH:mm}")).ToList(),
            employees.OrderBy(x => x.Apellidos).Select(x => new TripulacionOptionDto(x.EmpleadoId, $"{x.Nombres} {x.Apellidos} - {x.NumeroDocumento}")).ToList());
    }
}
