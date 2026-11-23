using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.TicketStatus.Commands;
using Sky.Application.UseCases.TicketStatus.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/ticket-statuses")]
public sealed class TicketStatusesController(ISender sender) : ControllerBase
{
    public sealed record CreateTicketStatusRequest(string StatusName, string? Description);
    public sealed record UpdateTicketStatusRequest(string StatusName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateTicketStatus(CreateTicketStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateTicketStatusCommand(body.StatusName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{ticketStatusId:int}")]
    public async Task<IActionResult> DeleteTicketStatus(int ticketStatusId, CancellationToken ct)
        => await sender.Send(new DeleteTicketStatusCommand(ticketStatusId), ct) ? NoContent() : NotFound();
    [HttpPut("{ticketStatusId:int}")]
    public async Task<IActionResult> UpdateTicketStatus(int ticketStatusId, UpdateTicketStatusRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTicketStatusCommand(ticketStatusId, body.StatusName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{ticketStatusId:int}")]
    public async Task<IActionResult> GetTicketStatusById(int ticketStatusId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTicketStatusByIdQuery(ticketStatusId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetTicketStatuses(CancellationToken ct)
        => Ok(await sender.Send(new GetTicketStatusesQuery(), ct));
}
