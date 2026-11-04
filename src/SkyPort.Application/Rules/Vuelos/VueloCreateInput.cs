using SkyPort.Application.Common;

namespace SkyPort.Application.Rules.Vuelos;

public sealed record VueloCreateInput(
    string NumeroVuelo,
    int RutaId,
    int AeronaveId,
    int? PuertaId,
    DateTime FechaSalida,
    DateTime FechaLlegada)
{
    public static Result<VueloCreateInput> Create(
        string? numeroVuelo,
        int rutaId,
        int aeronaveId,
        int? puertaId,
        DateTime fechaSalida,
        DateTime fechaLlegada)
    {
        var flightNumber = numeroVuelo?.Trim() ?? string.Empty;
        if (flightNumber.Length is < 1 or > 10 || rutaId <= 0 || aeronaveId <= 0 || puertaId is <= 0)
        {
            return Result<VueloCreateInput>.Failure(Error.Validation("Revisa el número de vuelo y las referencias seleccionadas."));
        }

        if (fechaLlegada <= fechaSalida)
        {
            return Result<VueloCreateInput>.Failure(Error.Validation("La llegada debe ser posterior a la salida."));
        }

        return Result<VueloCreateInput>.Success(new VueloCreateInput(
            flightNumber,
            rutaId,
            aeronaveId,
            puertaId,
            fechaSalida,
            fechaLlegada));
    }
}
