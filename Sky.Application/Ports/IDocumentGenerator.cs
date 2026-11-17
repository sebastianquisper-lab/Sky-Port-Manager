using Sky.Domain.Entities;

namespace Sky.Application.Ports;

public interface IDocumentGenerator
{
    /// <summary>PDF del boleto con todos los datos ya cargados por el caso de uso.</summary>
    Task<byte[]> GenerateTicketPdfAsync(
        Ticket ticket,
        Passenger passenger,
        Booking booking,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        CancellationToken ct = default);

    /// <summary>PDF del pase de abordar. <paramref name="departureGate"/> es null si el vuelo aún no tiene puerta.</summary>
    Task<byte[]> GenerateBoardingPassPdfAsync(
        BoardingPass boardingPass,
        Ticket ticket,
        Passenger passenger,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        BoardingGate? departureGate,
        CancellationToken ct = default);
}
