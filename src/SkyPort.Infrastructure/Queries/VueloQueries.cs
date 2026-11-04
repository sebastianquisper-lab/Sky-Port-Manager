using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Vuelos;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class VueloQueries(SkyPortContext context) : IVueloQueries
{
    public async Task<IReadOnlyList<VueloListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Vuelo
            .AsNoTracking()
            .OrderBy(flight => flight.FechaSalida)
            .ThenBy(flight => flight.NumeroVuelo)
            .ThenBy(flight => flight.VueloId)
            .Select(flight => new VueloListItemDto(
                flight.VueloId,
                flight.NumeroVuelo,
                flight.Ruta.CodigoOrigen + " · " + flight.Ruta.CiudadOrigen + " → " +
                flight.Ruta.CodigoDestino + " · " + flight.Ruta.CiudadDestino,
                flight.Aeronave.Matricula,
                flight.Puerta == null ? null : flight.Puerta.Codigo,
                flight.FechaSalida,
                flight.FechaLlegada,
                flight.Estado))
            .ToListAsync(cancellationToken);
}
