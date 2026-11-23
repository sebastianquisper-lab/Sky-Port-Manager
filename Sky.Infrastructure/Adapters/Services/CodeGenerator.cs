using System.Security.Cryptography;
using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

/// <summary>
/// Genera códigos aleatorios. La unicidad real la garantizan los índices únicos de la BD
/// (si hubiera una colisión, el INSERT falla con DbUpdateException).
/// </summary>
public sealed class CodeGenerator : ICodeGenerator
{
    // Sin 0/O/1/I para evitar confusiones al dictar el PNR.
    private const string PnrAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string GenerateBookingCode() => RandomFrom(PnrAlphabet, 6);

    public string GenerateTicketNumber(string airlineIataCode)
        => $"{Clean(airlineIataCode)}{DateTime.UtcNow:yyMMdd}{RandomDigits(10)}"; // máx. 3 + 6 + 10 = 19

    public string GenerateBaggageTagNumber(string airlineIataCode)
        => $"{Clean(airlineIataCode)}{RandomDigits(10)}"; // máx. 3 + 10 = 13

    private static string Clean(string iata)
        => new string((iata ?? string.Empty).Trim().ToUpperInvariant().Where(char.IsLetterOrDigit).Take(3).ToArray());

    private static string RandomDigits(int length) => RandomFrom("0123456789", length);

    private static string RandomFrom(string alphabet, int length)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
        }
        return new string(chars);
    }
}
