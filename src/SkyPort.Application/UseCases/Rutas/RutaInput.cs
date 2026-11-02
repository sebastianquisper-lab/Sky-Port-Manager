using SkyPort.Application.Common;

namespace SkyPort.Application.UseCases.Rutas;

internal sealed record RutaInput(
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
        var originCode = codigoOrigen?.Trim().ToUpperInvariant();
        var destinationCode = codigoDestino?.Trim().ToUpperInvariant();
        var originCity = ciudadOrigen?.Trim();
        var destinationCity = ciudadDestino?.Trim();

        if (!IsAirportCode(originCode) || !IsAirportCode(destinationCode))
        {
            return Result<RutaInput>.Failure(Error.Validation("Los códigos de origen y destino deben tener 3 letras."));
        }

        if (originCode == destinationCode)
        {
            return Result<RutaInput>.Failure(Error.Validation("El origen y el destino deben ser distintos."));
        }

        if (string.IsNullOrWhiteSpace(originCity) || originCity.Length > 80 ||
            string.IsNullOrWhiteSpace(destinationCity) || destinationCity.Length > 80)
        {
            return Result<RutaInput>.Failure(Error.Validation("Ingresa las ciudades de origen y destino (máximo 80 caracteres)."));
        }

        if (duracionMinutos <= 0)
        {
            return Result<RutaInput>.Failure(Error.Validation("La duración debe ser mayor a cero minutos."));
        }

        return Result<RutaInput>.Success(new RutaInput(
            originCode!,
            originCity,
            destinationCode!,
            destinationCity,
            duracionMinutos));
    }

    private static bool IsAirportCode(string? code) =>
        code is { Length: 3 } && code.All(char.IsAsciiLetter);
}
