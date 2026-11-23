using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Baggage.Commands;
using Sky.Application.UseCases.Baggage.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/baggages")]
public sealed class BaggagesController(ISender sender) : ControllerBase
{
    public sealed record CreateBaggageRequest(int TicketId, int BaggageTypeId, int BaggageStatusId, decimal WeightKg, string? Description);
    public sealed record UpdateBaggageRequest(int TicketId, int BaggageTypeId, int BaggageStatusId, decimal WeightKg, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateBaggage(CreateBaggageRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBaggageCommand(body.TicketId, body.BaggageTypeId, body.BaggageStatusId, body.WeightKg, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{baggageId:int}")]
    public async Task<IActionResult> DeleteBaggage(int baggageId, CancellationToken ct)
        => await sender.Send(new DeleteBaggageCommand(baggageId), ct) ? NoContent() : NotFound();
    [HttpPut("{baggageId:int}")]
    public async Task<IActionResult> UpdateBaggage(int baggageId, UpdateBaggageRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBaggageCommand(baggageId, body.TicketId, body.BaggageTypeId, body.BaggageStatusId, body.WeightKg, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{baggageId:int}")]
    public async Task<IActionResult> GetBaggageById(int baggageId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBaggageByIdQuery(baggageId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBaggages(CancellationToken ct)
        => Ok(await sender.Send(new GetBaggagesQuery(), ct));
}
