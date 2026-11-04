using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class BoletoQueries(SkyPortContext context) : IBoletoQueries
{
    public async Task<PagedResult<BoletoDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Query();
        var total = await query.CountAsync(cancellationToken);
        var pages = (int)Math.Ceiling(total / (double)pageSize);
        var page = pages == 0 ? 1 : Math.Min(pageNumber, pages);
        var items = await query.OrderByDescending(x => x.FechaEmision).ThenByDescending(x => x.BoletoId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<BoletoDto>(items, page, pageSize, total);
    }

    public Task<BoletoDto?> GetByIdAsync(int boletoId, CancellationToken cancellationToken = default) =>
        Query().FirstOrDefaultAsync(x => x.BoletoId == boletoId, cancellationToken);

    private IQueryable<BoletoDto> Query() => context.Boleto.AsNoTracking().Select(b => new BoletoDto(
        b.BoletoId, b.ReservaId, b.CodigoBoleto, b.FechaEmision, b.Estado, b.Reserva.Codigo,
        b.Reserva.Pasajero.Nombres + " " + b.Reserva.Pasajero.Apellidos,
        b.Reserva.Vuelo.NumeroVuelo, b.Reserva.Asiento));
}
