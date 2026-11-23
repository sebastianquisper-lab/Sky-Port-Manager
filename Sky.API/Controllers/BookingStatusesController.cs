using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BookingStatus.Commands;
using Sky.Application.UseCases.BookingStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/booking-statuses")]
public sealed class BookingStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreateBookingStatusRequest(string StatusName, string? Description);
    public sealed record UpdateBookingStatusRequest(string StatusName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateBookingStatus(CreateBookingStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBookingStatusCommand(body.StatusName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{bookingStatusId:int}")]
    public async Task<IActionResult> DeleteBookingStatus(int bookingStatusId, CancellationToken ct)
        => await sender.Send(new DeleteBookingStatusCommand(bookingStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{bookingStatusId:int}")]
    public async Task<IActionResult> UpdateBookingStatus(int bookingStatusId, UpdateBookingStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBookingStatusCommand(bookingStatusId, body.StatusName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{bookingStatusId:int}")]
    public async Task<IActionResult> GetBookingStatusById(int bookingStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBookingStatusByIdQuery(bookingStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBookingStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetBookingStatusesQuery(), ct));
}
