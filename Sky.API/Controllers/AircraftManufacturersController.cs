using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AircraftManufacturer.Commands;
using Sky.Application.UseCases.AircraftManufacturer.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/aircraft-manufacturers")]
public sealed class AircraftManufacturersController(ISender sender) : ControllerBase
{
    public sealed record CreateAircraftManufacturerRequest(string ManufacturerName);
    public sealed record UpdateAircraftManufacturerRequest(string ManufacturerName);

    [HttpPost]
    public async Task<IActionResult> CreateAircraftManufacturer(CreateAircraftManufacturerRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAircraftManufacturerCommand(body.ManufacturerName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{manufacturerId:int}")]
    public async Task<IActionResult> DeleteAircraftManufacturer(int manufacturerId, CancellationToken ct)
        => await sender.Send(new DeleteAircraftManufacturerCommand(manufacturerId), ct) ? NoContent() : NotFound();
    [HttpPut("{manufacturerId:int}")]
    public async Task<IActionResult> UpdateAircraftManufacturer(int manufacturerId, UpdateAircraftManufacturerRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAircraftManufacturerCommand(manufacturerId, body.ManufacturerName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{manufacturerId:int}")]
    public async Task<IActionResult> GetAircraftManufacturerById(int manufacturerId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAircraftManufacturerByIdQuery(manufacturerId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAircraftManufacturers(CancellationToken ct)
        => Ok(await sender.Send(new GetAircraftManufacturersQuery(), ct));
}
