using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Ticket.Commands;
using Sky.Application.UseCases.Ticket.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/tickets")]
public sealed class TicketsController(ISender sender) : ControllerBase
{
    public sealed record CreateTicketRequest(int BookingId, int FlightId, int PassengerId, int TicketStatusId, int CabinClassId, int? FareId, string? SeatNumber, decimal Price, decimal TaxAmount, string CurrencyCode);
    public sealed record UpdateTicketRequest(int BookingId, int FlightId, int PassengerId, int TicketStatusId, int CabinClassId, int? FareId, string? SeatNumber, decimal Price, decimal TaxAmount, string CurrencyCode);

    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTicketRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateTicketCommand(body.BookingId, body.FlightId, body.PassengerId, body.TicketStatusId, body.CabinClassId, body.FareId, body.SeatNumber, body.Price, body.TaxAmount, body.CurrencyCode), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{ticketId:int}")]
    public async Task<IActionResult> DeleteTicket(int ticketId, CancellationToken ct)
        => await sender.Send(new DeleteTicketCommand(ticketId), ct) ? NoContent() : NotFound();
    [HttpPut("{ticketId:int}")]
    public async Task<IActionResult> UpdateTicket(int ticketId, UpdateTicketRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTicketCommand(ticketId, body.BookingId, body.FlightId, body.PassengerId, body.TicketStatusId, body.CabinClassId, body.FareId, body.SeatNumber, body.Price, body.TaxAmount, body.CurrencyCode), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{ticketId:int}")]
    public async Task<IActionResult> GetTicketById(int ticketId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTicketByIdQuery(ticketId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{ticketId:int}/pdf")]
    public async Task<IActionResult> GetTicketPdf(int ticketId, CancellationToken ct)
    {
        var file = await sender.Send(new GetTicketPdfQuery(ticketId), ct);
        return File(file.Content, file.ContentType, file.FileName);
    }
    [HttpGet]
    public async Task<IActionResult> GetTickets(CancellationToken ct)
        => Ok(await sender.Send(new GetTicketsQuery(), ct));
}
