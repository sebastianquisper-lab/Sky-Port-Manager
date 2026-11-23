using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Terminal.Commands;
using Sky.Application.UseCases.Terminal.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/terminals")]
public sealed class TerminalsController(ISender sender) : ControllerBase
{
    public sealed record CreateTerminalRequest(int AirportId, string TerminalCode, string? TerminalName);
    public sealed record UpdateTerminalRequest(int AirportId, string TerminalCode, string? TerminalName, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateTerminal(CreateTerminalRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateTerminalCommand(body.AirportId, body.TerminalCode, body.TerminalName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{terminalId:int}")]
    public async Task<IActionResult> DeleteTerminal(int terminalId, CancellationToken ct)
        => await sender.Send(new DeleteTerminalCommand(terminalId), ct) ? NoContent() : NotFound();
    [HttpPut("{terminalId:int}")]
    public async Task<IActionResult> UpdateTerminal(int terminalId, UpdateTerminalRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTerminalCommand(terminalId, body.AirportId, body.TerminalCode, body.TerminalName, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{terminalId:int}")]
    public async Task<IActionResult> GetTerminalById(int terminalId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTerminalByIdQuery(terminalId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetTerminals(CancellationToken ct)
        => Ok(await sender.Send(new GetTerminalsQuery(), ct));
}
