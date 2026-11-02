using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Web.Models;

public sealed class VuelosIndexViewModel
{
    public DateTime? Desde { get; set; }

    public DateTime? Hasta { get; set; }

    public string? Estado { get; set; }

    public int? RutaId { get; set; }

    public int? AerolineaId { get; set; }

    public string? NumeroVuelo { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int Total { get; set; }

    public IReadOnlyList<VueloListItemDto> Vuelos { get; set; } = Array.Empty<VueloListItemDto>();

    public IReadOnlyList<RutaLookupDto> Rutas { get; set; } = Array.Empty<RutaLookupDto>();

    public IReadOnlyList<AerolineaLookupDto> Aerolineas { get; set; } = Array.Empty<AerolineaLookupDto>();
}
