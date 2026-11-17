namespace Sky.Application.Ports;

public interface ICodeGenerator
{
    /// <summary>Código de reserva (PNR) de hasta 20 caracteres. Debe ser único.</summary>
    string GenerateBookingCode();

    /// <summary>Número de boleto de hasta 30 caracteres. Debe ser único.</summary>
    string GenerateTicketNumber(string airlineIataCode);

    /// <summary>Etiqueta de equipaje de hasta 20 caracteres. Debe ser única.</summary>
    string GenerateBaggageTagNumber(string airlineIataCode);
}
