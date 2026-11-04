using SkyPort.Application.Common;

namespace SkyPort.Application.Rules.Rutas;

public sealed record RutaInput(
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int DuracionMinutos)
{
    public static Result<RutaInput> Create(
        string? codigoOrigen,
        string? ciudadOrigen,
        string? codigoDestino,
        string? ciudadDestino,
        int duracionMinutos)
    {
        var originCode = codigoOrigen?.Trim().ToUpperInvariant() ?? string.Empty;
        var originCity = ciudadOrigen?.Trim() ?? string.Empty;
        var destinationCode = codigoDestino?.Trim().ToUpperInvariant() ?? string.Empty;
        var destinationCity = ciudadDestino?.Trim() ?? string.Empty;

        if (originCode.Length is < 1 or > 3 || destinationCode.Length is < 1 or > 3 ||
            originCity.Length is < 1 or > 80 || destinationCity.Length is < 1 or > 80 ||
            duracionMinutos <= 0)
        {
            return Result<RutaInput>.Failure(Error.Validation("Revisa los códigos, ciudades y duración de la ruta."));
        }

        if (string.Equals(originCode, destinationCode, StringComparison.OrdinalIgnoreCase))
        {
            return Result<RutaInput>.Failure(Error.Validation("El origen y el destino deben ser distintos."));
        }

        return Result<RutaInput>.Success(new RutaInput(
            originCode,
            originCity,
            destinationCode,
            destinationCity,
            duracionMinutos));
    }
}
