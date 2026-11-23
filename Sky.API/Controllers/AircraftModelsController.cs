using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftModel.Commands;
using Sky.Application.UseCases.AircraftModel.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-models")]
public sealed class AircraftModelsController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftModelRequest(int ManufacturerId, string ModelName, string? Description);
    public sealed record UpdateAircraftModelRequest(int ManufacturerId, string ModelName, string? Description);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftModel(CreateAircraftModelRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftModelCommand(body.ManufacturerId, body.ModelName, body.Description), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{modelId:int}")]
    public async Task<IActionResult> DeleteAircraftModel(int modelId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftModelCommand(modelId), ct) ? NoContent() : NotFound();
    [HttpPut("{modelId:int}")]
    public async Task<IActionResult> UpdateAircraftModel(int modelId, UpdateAircraftModelRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftModelCommand(modelId, body.ManufacturerId, body.ModelName, body.Description), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{modelId:int}")]
    public async Task<IActionResult> GetAircraftModelById(int modelId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftModelByIdQuery(modelId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftModels(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftModelsQuery(), ct));
}
