using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Equipajes;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class EquipajeQueries(SkyPortContext context) : IEquipajeQueries
{
    public async Task<IReadOnlyList<EquipajeDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Equipaje
            .AsNoTracking()
            .OrderBy(equipaje => equipaje.Codigo)
            .ThenBy(equipaje => equipaje.EquipajeId)
            .Select(equipaje => new EquipajeDto(
                equipaje.EquipajeId,
                equipaje.BoletoId,
                equipaje.Boleto.CodigoBoleto,
                equipaje.Codigo,
                equipaje.PesoKg,
                equipaje.Estado))
            .ToListAsync(cancellationToken);

    public Task<EquipajeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Equipaje
            .AsNoTracking()
            .Where(equipaje => equipaje.EquipajeId == id)
            .Select(equipaje => new EquipajeDto(
                equipaje.EquipajeId,
                equipaje.BoletoId,
                equipaje.Boleto.CodigoBoleto,
                equipaje.Codigo,
                equipaje.PesoKg,
                equipaje.Estado))
            .FirstOrDefaultAsync(cancellationToken);
}
