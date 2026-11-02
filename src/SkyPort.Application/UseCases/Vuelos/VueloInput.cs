using System.Text.RegularExpressions;
using SkyPort.Application.Common;

namespace SkyPort.Application.UseCases.Vuelos;

internal sealed record VueloInput(
    string NumeroVuelo,
    int RutaId,
    int AeronaveId,
    int? PuertaId,
    DateTime FechaSalida,
    DateTime FechaLlegada)
{
    private static readonly Regex FlightNumberPattern = new("^[A-Z]{2}[0-9]{3,4}$", RegexOptions.Compiled);

    public static Result<VueloInput> Create(
        string? numeroVuelo,
        int rutaId,
        int aeronaveId,
        int? puertaId,
        DateTime fechaSalida,
        DateTime fechaLlegada)
    {
        var normalizedNumber = numeroVuelo?.Trim().ToUpperInvariant();
        if (normalizedNumber is null || !FlightNumberPattern.IsMatch(normalizedNumber))
        {
            return Result<VueloInput>.Failure(
                Error.Validation("El número de vuelo debe tener 2 letras seguidas de 3 o 4 dígitos."));
        }

        if (rutaId <= 0 || aeronaveId <= 0 || puertaId is <= 0)
        {
            return Result<VueloInput>.Failure(Error.Validation("Selecciona una ruta y una aeronave válidas."));
        }

        if (fechaLlegada <= fechaSalida)
        {
            return Result<VueloInput>.Failure(Error.Validation("La llegada debe ser posterior a la salida."));
        }

        return Result<VueloInput>.Success(new VueloInput(
            normalizedNumber,
            rutaId,
            aeronaveId,
            puertaId,
            fechaSalida,
            fechaLlegada));
    }
}
