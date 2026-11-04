using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class CheckInQueries(SkyPortContext context) : ICheckInQueries
{
    public async Task<PagedResult<CheckInDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Query();
        var total = await query.CountAsync(cancellationToken);
        var pages = (int)Math.Ceiling(total / (double)pageSize);
        var page = pages == 0 ? 1 : Math.Min(pageNumber, pages);
        var items = await query.OrderByDescending(x => x.FechaHora).ThenByDescending(x => x.CheckInId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<CheckInDto>(items, page, pageSize, total);
    }

    public Task<CheckInDto?> GetByIdAsync(int checkInId, CancellationToken cancellationToken = default) =>
        Query().FirstOrDefaultAsync(x => x.CheckInId == checkInId, cancellationToken);

    private IQueryable<CheckInDto> Query() => context.Checkin.AsNoTracking().Select(c => new CheckInDto(
        c.CheckInId, c.BoletoId, c.Boleto.CodigoBoleto, c.Boleto.ReservaId, c.Boleto.Reserva.Codigo,
        c.Boleto.Reserva.Pasajero.Nombres + " " + c.Boleto.Reserva.Pasajero.Apellidos,
        c.EmpleadoId, c.Empleado == null ? null : c.Empleado.Nombres + " " + c.Empleado.Apellidos,
        c.FechaHora, c.DocumentoVerificado));
}
