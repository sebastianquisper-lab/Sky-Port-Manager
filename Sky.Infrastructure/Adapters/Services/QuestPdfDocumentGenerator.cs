using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sky.Application.Ports;
using Sky.Domain.Entities;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class QuestPdfDocumentGenerator : IDocumentGenerator
{
    static QuestPdfDocumentGenerator()
    {
        // Licencia Community: gratuita para proyectos académicos y empresas pequeñas.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateTicketPdfAsync(
        Ticket ticket,
        Passenger passenger,
        Booking booking,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        CancellationToken ct = default)
    {
        var pdf = Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(28);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Column(col =>
            {
                col.Item().Text($"{airline.AirlineName} - Boleto electrónico").Bold().FontSize(18);
                col.Item().Text($"N.° de boleto: {ticket.TicketNumber}").FontSize(10);
            });

            page.Content().PaddingVertical(12).Column(col =>
            {
                col.Spacing(4);
                Field(col, "Pasajero", $"{passenger.LastName}/{passenger.FirstName}");
                Field(col, "Reserva (PNR)", booking.BookingCode);
                Field(col, "Vuelo", $"{airline.IataCode} {flight.FlightNumber}");
                Field(col, "Origen", $"{origin.AirportName} ({origin.IataCode})");
                Field(col, "Destino", $"{destination.AirportName} ({destination.IataCode})");
                Field(col, "Salida", FormatUtc(flight.ScheduledDepartureUtc));
                Field(col, "Llegada", FormatUtc(flight.ScheduledArrivalUtc));
                Field(col, "Clase", $"{cabinClass.ClassName} ({cabinClass.ClassCode})");
                Field(col, "Asiento", ticket.SeatNumber ?? "Por asignar");
                Field(col, "Tarifa", $"{ticket.Price:N2} {ticket.CurrencyCode}");
                Field(col, "Impuestos", $"{ticket.TaxAmount:N2} {ticket.CurrencyCode}");
                Field(col, "Emitido", FormatUtc(ticket.IssueDateUtc));
            });

            page.Footer().AlignCenter().Text("Presente este boleto junto con su documento de identidad.").FontSize(9);
        })).GeneratePdf();

        return Task.FromResult(pdf);
    }

    public Task<byte[]> GenerateBoardingPassPdfAsync(
        BoardingPass boardingPass,
        Ticket ticket,
        Passenger passenger,
        CabinClass cabinClass,
        Flight flight,
        Airline airline,
        Airport origin,
        Airport destination,
        BoardingGate? departureGate,
        CancellationToken ct = default)
    {
        var pdf = Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(28);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Column(col =>
            {
                col.Item().Text($"{airline.AirlineName} - Pase de abordar").Bold().FontSize(18);
                col.Item().Text($"{origin.IataCode}  ->  {destination.IataCode}").FontSize(16).SemiBold();
            });

            page.Content().PaddingVertical(12).Column(col =>
            {
                col.Spacing(4);
                Field(col, "Pasajero", $"{passenger.LastName}/{passenger.FirstName}");
                Field(col, "Vuelo", $"{airline.IataCode} {flight.FlightNumber}");
                Field(col, "Salida", FormatUtc(flight.ScheduledDepartureUtc));
                Field(col, "Puerta", departureGate?.GateCode ?? "Por confirmar");
                Field(col, "Asiento", ticket.SeatNumber ?? "Por asignar");
                Field(col, "Clase", cabinClass.ClassName);
                Field(col, "Grupo de abordaje", boardingPass.BoardingGroup ?? "-");
                Field(col, "Secuencia", boardingPass.SequenceNumber?.ToString(CultureInfo.InvariantCulture) ?? "-");

                col.Item().PaddingTop(10).Text(boardingPass.Barcode).FontFamily(Fonts.CourierNew).FontSize(9);
            });

            page.Footer().AlignCenter().Text("El código de arriba es la cadena BCBP del pase; un lector PDF417 puede generarse a partir de ella.").FontSize(8);
        })).GeneratePdf();

        return Task.FromResult(pdf);
    }

    private static void Field(ColumnDescriptor col, string label, string value)
        => col.Item().Text(text =>
        {
            text.Span($"{label}: ").SemiBold();
            text.Span(value);
        });

    private static string FormatUtc(DateTime utc)
        => utc.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);
}
