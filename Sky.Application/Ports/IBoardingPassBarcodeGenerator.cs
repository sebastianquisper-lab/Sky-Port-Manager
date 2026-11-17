using Sky.Domain.Entities;

namespace Sky.Application.Ports;

public interface IBoardingPassBarcodeGenerator
{
    /// <summary>Contenido del código de barras (formato IATA BCBP), máximo 100 caracteres y único por pase.</summary>
    string Generate(
        Ticket ticket,
        Passenger passenger,
        Booking booking,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        short? sequenceNumber);
}
