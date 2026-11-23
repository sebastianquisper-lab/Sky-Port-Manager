using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BoardingGate.Commands;
using Sky.Application.UseCases.BoardingGate.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/boarding-gates")]
public sealed class BoardingGatesController(ISender sender) : ControllerBase
{
    public sealed record CreateBoardingGateRequest(int TerminalId, string GateCode, string GateType);
    public sealed record UpdateBoardingGateRequest(int TerminalId, string GateCode, string GateType, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateBoardingGate(CreateBoardingGateRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBoardingGateCommand(body.TerminalId, body.GateCode, body.GateType), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{gateId:int}")]
    public async Task<IActionResult> DeleteBoardingGate(int gateId, CancellationToken ct)
        => await sender.Send(new DeleteBoardingGateCommand(gateId), ct) ? NoContent() : NotFound();
    [HttpPut("{gateId:int}")]
    public async Task<IActionResult> UpdateBoardingGate(int gateId, UpdateBoardingGateRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBoardingGateCommand(gateId, body.TerminalId, body.GateCode, body.GateType, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{gateId:int}")]
    public async Task<IActionResult> GetBoardingGateById(int gateId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBoardingGateByIdQuery(gateId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBoardingGates(CancellationToken ct)
        => Ok(await sender.Send(new GetBoardingGatesQuery(), ct));
}
