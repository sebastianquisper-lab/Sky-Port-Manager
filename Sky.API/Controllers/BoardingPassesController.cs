using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BoardingPass.Commands;
using Sky.Application.UseCases.BoardingPass.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/boarding-passes")]
public sealed class BoardingPassesController(ISender sender) : ControllerBase
{
    public sealed record CreateBoardingPassRequest(int TicketId, string? BoardingGroup, short? SequenceNumber);
    public sealed record UpdateBoardingPassRequest(int TicketId, string? BoardingGroup, short? SequenceNumber, string Barcode);

    [HttpPost]
    public async Task<IActionResult> CreateBoardingPass(CreateBoardingPassRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBoardingPassCommand(body.TicketId, body.BoardingGroup, body.SequenceNumber), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{boardingPassId:int}")]
    public async Task<IActionResult> DeleteBoardingPass(int boardingPassId, CancellationToken ct)
        => await sender.Send(new DeleteBoardingPassCommand(boardingPassId), ct) ? NoContent() : NotFound();
    [HttpPut("{boardingPassId:int}")]
    public async Task<IActionResult> UpdateBoardingPass(int boardingPassId, UpdateBoardingPassRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBoardingPassCommand(boardingPassId, body.TicketId, body.BoardingGroup, body.SequenceNumber, body.Barcode), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{boardingPassId:int}")]
    public async Task<IActionResult> GetBoardingPassById(int boardingPassId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBoardingPassByIdQuery(boardingPassId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{boardingPassId:int}/pdf")]
    public async Task<IActionResult> GetBoardingPassPdf(int boardingPassId, CancellationToken ct)
    {
        var file = await sender.Send(new GetBoardingPassPdfQuery(boardingPassId), ct);
        return File(file.Content, file.ContentType, file.FileName);
    }
    [HttpGet]
    public async Task<IActionResult> GetBoardingPasses(CancellationToken ct)
        => Ok(await sender.Send(new GetBoardingPassesQuery(), ct));
}
