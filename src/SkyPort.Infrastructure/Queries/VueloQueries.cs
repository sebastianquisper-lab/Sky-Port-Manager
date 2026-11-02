using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Vuelos.Dtos;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class VueloQueries(SkyPortContext context) : IVueloQueries
{
    public async Task<PagedResult<VueloListItemDto>> SearchAsync(
        VueloFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = context.Vuelo
            .AsNoTracking()
            .Include(flight => flight.Ruta)
            .Include(flight => flight.Aeronave)
                .ThenInclude(aircraft => aircraft.Aerolinea)
            .Include(flight => flight.Puerta)
            .AsQueryable();

        if (filter.Desde.HasValue)
        {
            query = query.Where(flight => flight.FechaSalida >= filter.Desde.Value);
        }

        if (filter.Hasta.HasValue)
        {
            query = query.Where(flight => flight.FechaSalida <= filter.Hasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Estado))
        {
            var state = filter.Estado.Trim();
            query = query.Where(flight => flight.Estado == state);
        }

        if (filter.RutaId.HasValue)
        {
            query = query.Where(flight => flight.RutaId == filter.RutaId.Value);
        }

        if (filter.AerolineaId.HasValue)
        {
            query = query.Where(flight => flight.Aeronave.AerolineaId == filter.AerolineaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.NumeroVuelo))
        {
            var flightNumber = filter.NumeroVuelo.Trim().ToUpperInvariant();
            query = query.Where(flight => flight.NumeroVuelo.Contains(flightNumber));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(flight => flight.FechaSalida)
            .ThenBy(flight => flight.NumeroVuelo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(flight => new VueloListItemDto(
                flight.VueloId,
                flight.NumeroVuelo,
                flight.Ruta.CodigoOrigen + " - " + flight.Ruta.CodigoDestino,
                flight.Aeronave.Aerolinea.Nombre,
                flight.FechaSalida,
                flight.FechaLlegada,
                flight.Estado,
                flight.Puerta == null ? null : flight.Puerta.Codigo,
                flight.Reserva.Count(reservation => reservation.Estado == EstadoReserva.Confirmada)))
            .ToListAsync(cancellationToken);

        return new PagedResult<VueloListItemDto>(items, page, pageSize, total);
    }

    public async Task<VueloDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Vuelo
            .AsNoTracking()
            .Where(flight => flight.VueloId == id)
            .Include(flight => flight.Ruta)
            .Include(flight => flight.Aeronave)
                .ThenInclude(aircraft => aircraft.Aerolinea)
            .Include(flight => flight.Puerta)
                .ThenInclude(gate => gate!.Terminal)
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
                flight.Aeronave.AerolineaId,
                flight.Aeronave.Aerolinea.Nombre,
                flight.Aeronave.Capacidad,
                flight.PuertaId,
                flight.Puerta == null ? null : flight.Puerta.Codigo,
                flight.Puerta == null ? null : flight.Puerta.Terminal.Nombre,
                flight.FechaSalida,
                flight.FechaLlegada,
                flight.Estado,
                flight.Reserva.Count(reservation => reservation.Estado == EstadoReserva.Confirmada)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VueloLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        var routes = await context.Ruta
            .AsNoTracking()
            .OrderBy(route => route.CodigoOrigen)
            .ThenBy(route => route.CodigoDestino)
            .Select(route => new RutaLookupDto(
                route.RutaId,
                route.CodigoOrigen,
                route.CiudadOrigen,
                route.CodigoDestino,
                route.CiudadDestino))
            .ToListAsync(cancellationToken);

        var airlines = await context.Aerolinea
            .AsNoTracking()
            .OrderBy(airline => airline.Nombre)
            .Select(airline => new AerolineaLookupDto(airline.AerolineaId, airline.Nombre))
            .ToListAsync(cancellationToken);

        var aircraft = await context.Aeronave
            .AsNoTracking()
            .Where(item => item.Estado == EstadoAeronave.Operativa)
            .OrderBy(item => item.Matricula)
            .Select(item => new AeronaveLookupDto(
                item.AeronaveId,
                item.Matricula,
                item.Modelo,
                item.Aerolinea.Nombre,
                item.Capacidad))
            .ToListAsync(cancellationToken);

        var gates = await context.Puerta
            .AsNoTracking()
            .Include(gate => gate.Terminal)
            .OrderBy(gate => gate.Terminal.Nombre)
            .ThenBy(gate => gate.Codigo)
            .Select(gate => new PuertaLookupDto(
                gate.PuertaId,
                gate.Terminal.Nombre,
                gate.Codigo))
            .ToListAsync(cancellationToken);

        return new VueloLookupsDto(routes, airlines, aircraft, gates);
    }
}
