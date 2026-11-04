using Microsoft.EntityFrameworkCore;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Queries;

public sealed class PasajeroQueries(SkyPortContext context) : IPasajeroQueries
{
    public async Task<PagedResult<PasajeroDto>> GetPageAsync(
        string? busqueda,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Pasajero.AsNoTracking();
        var termino = busqueda?.Trim();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            query = query.Where(pasajero =>
                pasajero.NumeroDocumento.Contains(termino) ||
                pasajero.Nombres.Contains(termino) ||
                pasajero.Apellidos.Contains(termino));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var effectivePage = totalPages == 0 ? 1 : Math.Min(pageNumber, totalPages);
        var items = await query
            .OrderBy(pasajero => pasajero.Apellidos)
            .ThenBy(pasajero => pasajero.Nombres)
            .ThenBy(pasajero => pasajero.PasajeroId)
            .Skip((effectivePage - 1) * pageSize)
            .Take(pageSize)
            .Select(pasajero => new PasajeroDto(
                pasajero.PasajeroId,
                pasajero.TipoDocumento,
                pasajero.NumeroDocumento,
                pasajero.Nombres,
                pasajero.Apellidos,
                pasajero.FechaNacimiento,
                pasajero.Nacionalidad,
                pasajero.Email,
                pasajero.Telefono))
            .ToListAsync(cancellationToken);

        return new PagedResult<PasajeroDto>(items, effectivePage, pageSize, totalCount);
    }

    public Task<PasajeroDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Pasajero
            .AsNoTracking()
            .Where(pasajero => pasajero.PasajeroId == id)
            .Select(pasajero => new PasajeroDto(
                pasajero.PasajeroId,
                pasajero.TipoDocumento,
                pasajero.NumeroDocumento,
                pasajero.Nombres,
                pasajero.Apellidos,
                pasajero.FechaNacimiento,
                pasajero.Nacionalidad,
                pasajero.Email,
                pasajero.Telefono))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<PasajeroDto?> GetByDocumentoAsync(
        string tipoDocumento,
        string numeroDocumento,
        CancellationToken cancellationToken = default) =>
        context.Pasajero
            .AsNoTracking()
            .Where(pasajero =>
                pasajero.TipoDocumento == tipoDocumento && pasajero.NumeroDocumento == numeroDocumento)
            .Select(pasajero => new PasajeroDto(
                pasajero.PasajeroId,
                pasajero.TipoDocumento,
                pasajero.NumeroDocumento,
                pasajero.Nombres,
                pasajero.Apellidos,
                pasajero.FechaNacimiento,
                pasajero.Nacionalidad,
                pasajero.Email,
                pasajero.Telefono))
            .FirstOrDefaultAsync(cancellationToken);
}
