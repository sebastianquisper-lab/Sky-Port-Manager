using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Dashboard.Dtos;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class DashboardQueries(SkyPortContext context) : IDashboardQueries
{
    public async Task<DashboardDto> GetResumenAsync(CancellationToken cancellationToken = default)
    {
        var ahora = DateTime.Now;
        var inicioHoy = ahora.Date;
        var inicioManana = inicioHoy.AddDays(1);

        var vuelosHoyQuery = context.Vuelo
            .AsNoTracking()
            .Where(vuelo => vuelo.FechaSalida >= inicioHoy && vuelo.FechaSalida < inicioManana);

        var vuelosHoy = await vuelosHoyQuery.CountAsync(cancellationToken);
        var vuelosPorEstado = await vuelosHoyQuery
            .GroupBy(vuelo => vuelo.Estado)
            .Select(grupo => new VuelosPorEstadoDto(grupo.Key, grupo.Count()))
            .OrderBy(item => item.Estado)
            .ToListAsync(cancellationToken);

        var proximosVuelos = await context.Vuelo
            .AsNoTracking()
            .Where(vuelo => vuelo.FechaSalida >= ahora && vuelo.Estado != EstadoVuelo.Cancelado)
            .OrderBy(vuelo => vuelo.FechaSalida)
            .Take(5)
            .Select(vuelo => new ProximoVueloDto(
                vuelo.NumeroVuelo,
                vuelo.Ruta.CodigoOrigen,
                vuelo.Ruta.CodigoDestino,
                vuelo.FechaSalida,
                vuelo.Estado))
            .ToListAsync(cancellationToken);

        var reservasConfirmadasHoy = await context.Reserva
            .AsNoTracking()
            .CountAsync(reserva =>
                reserva.FechaReserva >= inicioHoy &&
                reserva.FechaReserva < inicioManana &&
                reserva.Estado == EstadoReserva.Confirmada,
                cancellationToken);

        var pasajerosAbordadosHoy = await context.Embarque
            .AsNoTracking()
            .CountAsync(embarque =>
                embarque.FechaHora >= inicioHoy && embarque.FechaHora < inicioManana,
                cancellationToken);

        return new DashboardDto(
            vuelosHoy,
            vuelosPorEstado,
            proximosVuelos,
            reservasConfirmadasHoy,
            pasajerosAbordadosHoy);
    }
}
