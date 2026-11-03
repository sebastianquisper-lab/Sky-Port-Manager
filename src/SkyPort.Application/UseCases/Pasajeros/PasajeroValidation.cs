using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;

namespace SkyPort.Application.UseCases.Pasajeros;

internal static partial class PasajeroValidation
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static Error? Validate(
        string tipoDocumento,
        string numeroDocumento,
        string nombres,
        string apellidos,
        DateOnly fechaNacimiento,
        string nacionalidad,
        string? email,
        string? telefono)
    {
        if (!IsDocumentValid(tipoDocumento, numeroDocumento))
        {
            return Error.Validation("El tipo o número de documento no tiene un formato válido.");
        }

        if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
        {
            return Error.Validation("Los nombres y apellidos son obligatorios.");
        }

        if (nombres.Trim().Length > 80 || apellidos.Trim().Length > 80)
        {
            return Error.Validation("Los nombres y apellidos no pueden superar 80 caracteres.");
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (fechaNacimiento > hoy || fechaNacimiento < hoy.AddYears(-120))
        {
            return Error.Validation("La fecha de nacimiento debe estar entre hoy y los últimos 120 años.");
        }

        if (!string.IsNullOrWhiteSpace(nacionalidad) && nacionalidad.Trim().Length > 60)
        {
            return Error.Validation("La nacionalidad no puede superar 60 caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            (email.Trim().Length > 120 || !EmailValidator.IsValid(email.Trim())))
        {
            return Error.Validation("El correo electrónico no tiene un formato válido.");
        }

        if (telefono?.Trim().Length > 20)
        {
            return Error.Validation("El teléfono no puede superar 20 caracteres.");
        }

        return null;
    }

    internal static bool IsDocumentValid(string tipoDocumento, string numeroDocumento)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento))
        {
            return false;
        }

        return tipoDocumento switch
        {
            TipoDocumento.DNI => DniRegex().IsMatch(numeroDocumento),
            TipoDocumento.Pasaporte => PassportRegex().IsMatch(numeroDocumento),
            TipoDocumento.CE => CeRegex().IsMatch(numeroDocumento),
            _ => false
        };
    }

    [GeneratedRegex("^[0-9]{8}$", RegexOptions.CultureInvariant)]
    private static partial Regex DniRegex();

    [GeneratedRegex("^[A-Za-z0-9]{6,12}$", RegexOptions.CultureInvariant)]
    private static partial Regex PassportRegex();

    [GeneratedRegex("^[A-Za-z0-9]{9,12}$", RegexOptions.CultureInvariant)]
    private static partial Regex CeRegex();
}
