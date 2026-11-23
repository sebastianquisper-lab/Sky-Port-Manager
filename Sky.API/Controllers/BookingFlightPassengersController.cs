using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BookingFlightPassenger.Commands;
using Sky.Application.UseCases.BookingFlightPassenger.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/booking-flight-passengers")]
public sealed class BookingFlightPassengersController(ISender sender) : ControllerBase
{
    public sealed record CreateBookingFlightPassengerRequest(int BookingId, int FlightId, int PassengerId);

    [HttpPost]
    public async Task<IActionResult> CreateBookingFlightPassenger(CreateBookingFlightPassengerRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBookingFlightPassengerCommand(body.BookingId, body.FlightId, body.PassengerId), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{bookingId:int}/{flightId:int}/{passengerId:int}")]
    public async Task<IActionResult> DeleteBookingFlightPassenger(int bookingId, int flightId, int passengerId, CancellationToken ct)
        => await sender.Send(new DeleteBookingFlightPassengerCommand(bookingId, flightId, passengerId), ct) ? NoContent() : NotFound();
    [HttpGet("{bookingId:int}/{flightId:int}/{passengerId:int}")]
    public async Task<IActionResult> GetBookingFlightPassengerById(int bookingId, int flightId, int passengerId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBookingFlightPassengerByIdQuery(bookingId, flightId, passengerId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBookingFlightPassengers(CancellationToken ct)
        => Ok(await sender.Send(new GetBookingFlightPassengersQuery(), ct));
}
