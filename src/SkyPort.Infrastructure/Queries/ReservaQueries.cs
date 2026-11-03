using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Reservas;
using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class ReservaQueries(SkyPortContext context) : IReservaQueries
{
    public async Task<IReadOnlyList<VueloAbiertoDto>> GetVuelosAbiertosAsync(
        CancellationToken cancellationToken = default)
    {
        var estadosAbiertos = new[] { EstadoVuelo.Programado, EstadoVuelo.Retrasado };

        return await context.Vuelo
            .AsNoTracking()
            .Where(vuelo => estadosAbiertos.Contains(vuelo.Estado))
            .OrderBy(vuelo => vuelo.FechaSalida)
            .Select(vuelo => new VueloAbiertoDto(
                vuelo.VueloId,
                vuelo.NumeroVuelo,
                vuelo.Ruta.CodigoOrigen,
                vuelo.Ruta.CiudadOrigen,
                vuelo.Ruta.CodigoDestino,
                vuelo.Ruta.CiudadDestino,
                vuelo.FechaSalida,
                vuelo.Estado,
                vuelo.Aeronave.Capacidad,
                vuelo.Reserva.Count(reserva => reserva.Estado == EstadoReserva.Confirmada),
                vuelo.Aeronave.Capacidad - vuelo.Reserva.Count(reserva => reserva.Estado == EstadoReserva.Confirmada)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReservaDto>> GetByVueloAsync(
        int vueloId,
        CancellationToken cancellationToken = default) =>
        await GetReservasQuery()
            .Where(reserva => reserva.VueloId == vueloId)
            .OrderBy(reserva => reserva.Asiento)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReservaDto>> GetByPasajeroAsync(
        int pasajeroId,
        CancellationToken cancellationToken = default) =>
        await GetReservasQuery()
            .Where(reserva => reserva.PasajeroId == pasajeroId)
            .OrderByDescending(reserva => reserva.FechaReserva)
            .ToListAsync(cancellationToken);

    public async Task<AsientosVueloDto?> GetAsientosAsync(
        int vueloId,
        CancellationToken cancellationToken = default)
    {
        var capacidad = await context.Vuelo
            .AsNoTracking()
            .Where(vuelo => vuelo.VueloId == vueloId)
            .Select(vuelo => (int?)vuelo.Aeronave.Capacidad)
            .FirstOrDefaultAsync(cancellationToken);

        if (capacidad is null)
        {
            return null;
        }

        var asientosOcupados = await context.Reserva
            .AsNoTracking()
            .Where(reserva => reserva.VueloId == vueloId && reserva.Estado == EstadoReserva.Confirmada)
            .OrderBy(reserva => reserva.Asiento)
            .Select(reserva => reserva.Asiento)
            .ToListAsync(cancellationToken);

        return new AsientosVueloDto(capacidad.Value, AsientoLayout.Filas(capacidad.Value), asientosOcupados);
    }

    private IQueryable<ReservaDto> GetReservasQuery() => context.Reserva
        .AsNoTracking()
        .Select(reserva => new ReservaDto(
            reserva.ReservaId,
            reserva.Codigo,
            reserva.PasajeroId,
            reserva.Pasajero.Nombres + " " + reserva.Pasajero.Apellidos,
            reserva.Pasajero.TipoDocumento,
            reserva.Pasajero.NumeroDocumento,
            reserva.VueloId,
            reserva.Vuelo.NumeroVuelo,
            reserva.Vuelo.Ruta.CodigoOrigen + " - " + reserva.Vuelo.Ruta.CodigoDestino,
            reserva.Vuelo.FechaSalida,
            reserva.Asiento,
            reserva.FechaReserva,
            reserva.Estado,
            reserva.Boleto == null ? null : reserva.Boleto.BoletoId,
            reserva.Boleto == null ? null : reserva.Boleto.Estado));
}
