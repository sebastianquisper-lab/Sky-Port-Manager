using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Boarding.Commands;
using Sky.Application.UseCases.Boarding.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/boardings")]
public sealed class BoardingsController(ISender sender) : ControllerBase
{
    public sealed record CreateBoardingRequest(int TicketId, int GateId, int? EmployeeId);
    public sealed record UpdateBoardingRequest(int TicketId, int GateId, int? EmployeeId);

    [HttpPost]
    public async Task<IActionResult> CreateBoarding(CreateBoardingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBoardingCommand(body.TicketId, body.GateId, body.EmployeeId), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{boardingId:int}")]
    public async Task<IActionResult> DeleteBoarding(int boardingId, CancellationToken ct)
        => await sender.Send(new DeleteBoardingCommand(boardingId), ct) ? NoContent() : NotFound();
    [HttpPut("{boardingId:int}")]
    public async Task<IActionResult> UpdateBoarding(int boardingId, UpdateBoardingRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBoardingCommand(boardingId, body.TicketId, body.GateId, body.EmployeeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{boardingId:int}")]
    public async Task<IActionResult> GetBoardingById(int boardingId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBoardingByIdQuery(boardingId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBoardings(CancellationToken ct)
        => Ok(await sender.Send(new GetBoardingsQuery(), ct));
}
