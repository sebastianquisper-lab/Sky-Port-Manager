using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Booking.Commands;
using Sky.Application.UseCases.Booking.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/bookings")]
public sealed class BookingsController(ISender sender) : ControllerBase
{
    public sealed record CreateBookingRequest(int BookingStatusId, DateTime? ExpirationDateUtc, string? ContactEmail, string? ContactPhone);
    public sealed record CreateBookingWithTicketsRequest(string? ContactEmail, string? ContactPhone, string CurrencyCode, int PaymentMethodId, string? PaymentToken, IReadOnlyList<BookingItem> Items);
    public sealed record UpdateBookingRequest(int BookingStatusId, string BookingCode, DateTime? ExpirationDateUtc, string? ContactEmail, string? ContactPhone);

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBookingCommand(body.BookingStatusId, body.ExpirationDateUtc, body.ContactEmail, body.ContactPhone), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpPost("create-booking-with-tickets")]
    public async Task<IActionResult> CreateBookingWithTickets(CreateBookingWithTicketsRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBookingWithTicketsCommand(body.ContactEmail, body.ContactPhone, body.CurrencyCode, body.PaymentMethodId, body.PaymentToken, body.Items), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{bookingId:int}")]
    public async Task<IActionResult> DeleteBooking(int bookingId, CancellationToken ct)
        => await sender.Send(new DeleteBookingCommand(bookingId), ct) ? NoContent() : NotFound();
    [HttpPut("{bookingId:int}")]
    public async Task<IActionResult> UpdateBooking(int bookingId, UpdateBookingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBookingCommand(bookingId, body.BookingStatusId, body.BookingCode, body.ExpirationDateUtc, body.ContactEmail, body.ContactPhone), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{bookingId:int}")]
    public async Task<IActionResult> GetBookingById(int bookingId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBookingByIdQuery(bookingId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBookings(CancellationToken ct)
        => Ok(await sender.Send(new GetBookingsQuery(), ct));
}
