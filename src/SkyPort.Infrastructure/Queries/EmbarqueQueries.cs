using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class EmbarqueQueries(SkyPortContext context) : IEmbarqueQueries
{
    public async Task<IReadOnlyList<EmbarqueListItemDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Embarque
            .AsNoTracking()
            .OrderByDescending(boarding => boarding.FechaHora)
            .ThenBy(boarding => boarding.EmbarqueId)
            .Select(boarding => new EmbarqueListItemDto(
                boarding.EmbarqueId,
                boarding.Boleto.CodigoBoleto,
                boarding.Boleto.Reserva.Pasajero.Nombres + " " + boarding.Boleto.Reserva.Pasajero.Apellidos,
                boarding.Boleto.Reserva.Vuelo.NumeroVuelo,
                boarding.Puerta == null ? null : boarding.Puerta.Codigo,
                boarding.FechaHora))
            .ToListAsync(cancellationToken);

    public Task<EmbarqueDetailDto?> GetByIdAsync(int embarqueId, CancellationToken cancellationToken) =>
        context.Embarque
            .AsNoTracking()
            .Where(boarding => boarding.EmbarqueId == embarqueId)
            .Select(boarding => new EmbarqueDetailDto(
                boarding.EmbarqueId,
                boarding.Boleto.CodigoBoleto,
                boarding.Boleto.Reserva.Pasajero.Nombres + " " + boarding.Boleto.Reserva.Pasajero.Apellidos,
                boarding.Boleto.Reserva.Vuelo.NumeroVuelo,
                boarding.Puerta == null ? null : boarding.Puerta.Codigo,
                boarding.FechaHora))
            .FirstOrDefaultAsync(cancellationToken);
}
