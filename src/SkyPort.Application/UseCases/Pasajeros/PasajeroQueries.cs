using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.UseCases.Pasajeros.Dtos;

namespace SkyPort.Application.UseCases.Pasajeros;

public sealed record GetPasajerosQuery(string? Busqueda = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<PasajeroDto>>;

public sealed record GetPasajeroByIdQuery(int PasajeroId) : IRequest<Result<PasajeroDto>>;

public sealed record GetPasajeroByDocumentoQuery(string TipoDocumento, string NumeroDocumento)
    : IRequest<Result<PasajeroDto>>;

public sealed class GetPasajerosQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajerosQuery, PagedResult<PasajeroDto>>
{
    public Task<PagedResult<PasajeroDto>> Handle(
        GetPasajerosQuery request,
        CancellationToken cancellationToken) =>
        queries.GetPageAsync(
            request.Busqueda,
            Math.Max(1, request.Page),
            Math.Clamp(request.PageSize, 1, 100),
            cancellationToken);
}

public sealed class GetPasajeroByIdQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajeroByIdQuery, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(
        GetPasajeroByIdQuery request,
        CancellationToken cancellationToken)
    {
        var pasajero = await queries.GetByIdAsync(request.PasajeroId, cancellationToken);
        return pasajero is null
            ? Result<PasajeroDto>.Failure(Error.NotFound("No se encontró el pasajero."))
            : Result<PasajeroDto>.Success(pasajero);
    }
}

public sealed class GetPasajeroByDocumentoQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajeroByDocumentoQuery, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(
        GetPasajeroByDocumentoQuery request,
        CancellationToken cancellationToken)
    {
        var tipoDocumento = request.TipoDocumento?.Trim() ?? string.Empty;
        var numeroDocumento = request.NumeroDocumento?.Trim() ?? string.Empty;

        if (!PasajeroValidation.IsDocumentValid(tipoDocumento, numeroDocumento))
        {
            return Result<PasajeroDto>.Failure(Error.Validation("El tipo o número de documento no tiene un formato válido."));
        }

        var pasajero = await queries.GetByDocumentoAsync(tipoDocumento, numeroDocumento, cancellationToken);
        return pasajero is null
            ? Result<PasajeroDto>.Failure(Error.NotFound("No se encontró un pasajero con ese documento."))
            : Result<PasajeroDto>.Success(pasajero);
    }
}
