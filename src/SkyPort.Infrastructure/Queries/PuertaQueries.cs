using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Dtos.Puertas;
using SkyPort.Application.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class PuertaQueries(SkyPortContext context) : IPuertaQueries
{
    public async Task<IReadOnlyList<PuertaDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Puerta
            .AsNoTracking()
            .OrderBy(puerta => puerta.Terminal.Nombre)
            .ThenBy(puerta => puerta.Codigo)
            .ThenBy(puerta => puerta.PuertaId)
            .Select(puerta => new PuertaDto(
                puerta.PuertaId,
                puerta.TerminalId,
                puerta.Terminal.Nombre,
                puerta.Codigo,
                puerta.Disponible))
            .ToListAsync(cancellationToken);

    public Task<PuertaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Puerta
            .AsNoTracking()
            .Where(puerta => puerta.PuertaId == id)
            .Select(puerta => new PuertaDto(
                puerta.PuertaId,
                puerta.TerminalId,
                puerta.Terminal.Nombre,
                puerta.Codigo,
                puerta.Disponible))
            .FirstOrDefaultAsync(cancellationToken);
}
