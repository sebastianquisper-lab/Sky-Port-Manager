using System.Globalization;
using System.Text.RegularExpressions;

namespace SkyPort.Application.UseCases.Reservas;

public static partial class AsientoLayout
{
    private const int AsientosPorFila = 6;
    private const string LetrasAsiento = "ABCDEF";

    public static int Filas(int capacidad) => capacidad <= 0
        ? 0
        : ((capacidad - 1) / AsientosPorFila) + 1;

    public static bool EsValido(string? asiento, int capacidad)
    {
        if (capacidad <= 0 || string.IsNullOrWhiteSpace(asiento))
        {
            return false;
        }

        var coincidencia = SeatRegex().Match(asiento.Trim().ToUpperInvariant());
        if (!coincidencia.Success ||
            !int.TryParse(coincidencia.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var fila))
        {
            return false;
        }

        var columna = LetrasAsiento.IndexOf(coincidencia.Groups[2].Value[0]);
        if (fila < 1 || columna < 0)
        {
            return false;
        }

        var numeroAsiento = ((long)fila - 1) * AsientosPorFila + columna + 1;
        return numeroAsiento <= capacidad;
    }

    [GeneratedRegex("^([1-9][0-9]*)([A-F])$", RegexOptions.CultureInvariant)]
    private static partial Regex SeatRegex();
}
