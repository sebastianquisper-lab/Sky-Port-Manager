using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sky.Application.Ports;
using Sky.Domain.Entities;

namespace Sky.Infrastructure.Adapters.Services;

/// <summary>
/// Cadena IATA BCBP simplificada (solo la sección obligatoria de un tramo): 61 caracteres,
/// por debajo del máximo de 100 de la columna. Es única por pase porque combina PNR, pasajero, vuelo y fecha.
/// </summary>
public sealed partial class BoardingPassBarcodeGenerator : IBoardingPassBarcodeGenerator
{
    public string Generate(
        Ticket ticket,
        Passenger passenger,
        Booking booking,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        short? sequenceNumber)
    {
        var name = Fit($"{passenger.LastName}/{passenger.FirstName}", 20);
        var pnr = Fit(booking.BookingCode, 7);
        var from = Fit(origin.IataCode, 3);
        var to = Fit(destination.IataCode, 3);
        var carrier = Fit(airline.IataCode, 3);
        var flightNumber = FlightNumberDigits(flight.FlightNumber).PadLeft(4, '0') + " ";
        var julianDate = flight.ScheduledDepartureUtc.DayOfYear.ToString("000", CultureInfo.InvariantCulture);
        var compartment = string.IsNullOrWhiteSpace(cabinClass.ClassCode) ? " " : cabinClass.ClassCode.Trim()[..1].ToUpperInvariant();
        var seat = string.IsNullOrWhiteSpace(ticket.SeatNumber) ? "    " : Fit(ticket.SeatNumber, 4, '0', padLeft: true);
        var sequence = (sequenceNumber ?? 0).ToString("0000", CultureInfo.InvariantCulture) + " ";

        // M + 1 tramo + nombre + e-ticket + PNR + origen + destino + aerolínea + vuelo + fecha juliana
        // + cabina + asiento + secuencia + estado (1 = con check-in) + tamaño del campo condicional (00).
        return $"M1{name}E{pnr}{from}{to}{carrier}{flightNumber}{julianDate}{compartment}{seat}{sequence}100";
    }

    private static string FlightNumberDigits(string flightNumber)
    {
        var match = TrailingDigits().Match(flightNumber ?? string.Empty);
        return match.Success ? match.Value : "0";
    }

    private static string Fit(string value, int length, char pad = ' ', bool padLeft = false)
    {
        var decomposed = (value ?? string.Empty).Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark) continue;
            var c = char.ToUpperInvariant(ch);
            if (c is (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '/' or ' ') sb.Append(c);
        }

        var clean = sb.ToString();
        if (clean.Length > length) clean = clean[..length];
        return padLeft ? clean.PadLeft(length, pad) : clean.PadRight(length, pad);
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex TrailingDigits();
}
