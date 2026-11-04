using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Rutas;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class RutaQueries(SkyPortContext context) : IRutaQueries
{
    public Task<RutaDto?> GetByIdAsync(int rutaId, CancellationToken cancellationToken) =>
        context.Ruta
            .AsNoTracking()
            .Where(route => route.RutaId == rutaId)
            .Select(route => new RutaDto(
                route.RutaId,
                route.CodigoOrigen,
                route.CiudadOrigen,
                route.CodigoDestino,
                route.CiudadDestino,
                route.DuracionMinutos))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> IsUsedByFlightsAsync(int rutaId, CancellationToken cancellationToken) =>
        context.Vuelo.AsNoTracking().AnyAsync(flight => flight.RutaId == rutaId, cancellationToken);
}
