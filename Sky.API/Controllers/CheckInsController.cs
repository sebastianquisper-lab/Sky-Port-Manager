using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.CheckIn.Commands;
using Sky.Application.UseCases.CheckIn.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/check-ins")]
public sealed class CheckInsController(ISender sender) : ControllerBase
{
    public sealed record CreateCheckInRequest(int TicketId, int? CounterId, int? EmployeeId, string Channel);
    public sealed record UpdateCheckInRequest(int TicketId, int? CounterId, int? EmployeeId, string Channel);

    [HttpPost]
    public async Task<IActionResult> CreateCheckIn(CreateCheckInRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCheckInCommand(body.TicketId, body.CounterId, body.EmployeeId, body.Channel), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{checkInId:int}")]
    public async Task<IActionResult> DeleteCheckIn(int checkInId, CancellationToken ct)
        => await sender.Send(new DeleteCheckInCommand(checkInId), ct) ? NoContent() : NotFound();
    [HttpPut("{checkInId:int}")]
    public async Task<IActionResult> UpdateCheckIn(int checkInId, UpdateCheckInRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCheckInCommand(checkInId, body.TicketId, body.CounterId, body.EmployeeId, body.Channel), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{checkInId:int}")]
    public async Task<IActionResult> GetCheckInById(int checkInId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCheckInByIdQuery(checkInId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCheckIns(CancellationToken ct)
        => Ok(await sender.Send(new GetCheckInsQuery(), ct));
}
