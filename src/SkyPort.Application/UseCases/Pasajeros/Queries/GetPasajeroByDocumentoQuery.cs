using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Pasajeros.Queries;

public sealed record GetPasajeroByDocumentoQuery(string TipoDocumento, string NumeroDocumento)
    : IRequest<Result<PasajeroDto>>;

public sealed class GetPasajeroByDocumentoQueryHandler(IPasajeroQueries queries)
    : IRequestHandler<GetPasajeroByDocumentoQuery, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(GetPasajeroByDocumentoQuery request, CancellationToken cancellationToken)
    {
        var tipoDocumento = request.TipoDocumento?.Trim() ?? string.Empty;
        var numeroDocumento = request.NumeroDocumento?.Trim() ?? string.Empty;
        if (!PasajeroValidation.IsDocumentValid(tipoDocumento, numeroDocumento))
            return Result<PasajeroDto>.Failure(Error.Validation("El tipo o número de documento no tiene un formato válido."));

        var pasajero = await queries.GetByDocumentoAsync(tipoDocumento, numeroDocumento, cancellationToken);
        return pasajero is null
            ? Result<PasajeroDto>.Failure(Error.NotFound("No se encontró un pasajero con ese documento."))
            : Result<PasajeroDto>.Success(pasajero);
    }
}
