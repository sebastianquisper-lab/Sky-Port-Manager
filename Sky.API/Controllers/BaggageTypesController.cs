using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.BaggageType.Commands;
using Sky.Application.UseCases.BaggageType.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/baggage-types")]
public sealed class BaggageTypesController(ISender sender) : ControllerBase
{
    public sealed record CreateBaggageTypeRequest(string TypeName, decimal? MaxWeightKg);
    public sealed record UpdateBaggageTypeRequest(string TypeName, decimal? MaxWeightKg);

    [HttpPost]
    public async Task<IActionResult> CreateBaggageType(CreateBaggageTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateBaggageTypeCommand(body.TypeName, body.MaxWeightKg), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{baggageTypeId:int}")]
    public async Task<IActionResult> DeleteBaggageType(int baggageTypeId, CancellationToken ct)
        => await sender.Send(new DeleteBaggageTypeCommand(baggageTypeId), ct) ? NoContent() : NotFound();
    [HttpPut("{baggageTypeId:int}")]
    public async Task<IActionResult> UpdateBaggageType(int baggageTypeId, UpdateBaggageTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBaggageTypeCommand(baggageTypeId, body.TypeName, body.MaxWeightKg), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{baggageTypeId:int}")]
    public async Task<IActionResult> GetBaggageTypeById(int baggageTypeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetBaggageTypeByIdQuery(baggageTypeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetBaggageTypes(CancellationToken ct)
        => Ok(await sender.Send(new GetBaggageTypesQuery(), ct));
}
