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

    public Task<VueloDetailDto?> GetByIdAsync(int vueloId, CancellationToken cancellationToken) =>
        context.Vuelo
            .AsNoTracking()
            .Where(flight => flight.VueloId == vueloId)
            .Select(flight => new VueloDetailDto(
                flight.VueloId,
                flight.NumeroVuelo,
                flight.RutaId,
                flight.Ruta.CodigoOrigen,
                flight.Ruta.CiudadOrigen,
                flight.Ruta.CodigoDestino,
                flight.Ruta.CiudadDestino,
                flight.AeronaveId,
                flight.Aeronave.Matricula,
                flight.Aeronave.Modelo,
                flight.PuertaId,
                flight.Puerta == null ? null : flight.Puerta.Codigo,
                flight.FechaSalida,
                flight.FechaLlegada,
                flight.Estado))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<VueloLookupsDto> GetCreateOptionsAsync(CancellationToken cancellationToken)
    {
        var routes = await context.Ruta
            .AsNoTracking()
            .OrderBy(route => route.CodigoOrigen)
            .ThenBy(route => route.CodigoDestino)
            .Select(route => new RutaOpcionDto(
                route.RutaId,
                route.CodigoOrigen + " · " + route.CiudadOrigen + " → " + route.CodigoDestino + " · " + route.CiudadDestino))
            .ToListAsync(cancellationToken);

        var aircraft = await context.Aeronave
            .AsNoTracking()
            .OrderBy(item => item.Matricula)
            .Select(item => new AeronaveOpcionDto(item.AeronaveId, item.Matricula + " · " + item.Modelo))
            .ToListAsync(cancellationToken);

        var gates = await context.Puerta
            .AsNoTracking()
            .OrderBy(gate => gate.Terminal.Nombre)
            .ThenBy(gate => gate.Codigo)
            .Select(gate => new PuertaOpcionDto(gate.PuertaId, gate.Terminal.Nombre + " · " + gate.Codigo))
            .ToListAsync(cancellationToken);

        return new VueloLookupsDto(routes, aircraft, gates);
    }

    public async Task<bool> HasReservationsOrCrewAsync(int vueloId, CancellationToken cancellationToken)
    {
        if (await context.Reserva.AsNoTracking().AnyAsync(reservation => reservation.VueloId == vueloId, cancellationToken))
        {
            return true;
        }

        return await context.Tripulacionvuelo
            .AsNoTracking()
            .AnyAsync(assignment => assignment.VueloId == vueloId, cancellationToken);
    }
}
