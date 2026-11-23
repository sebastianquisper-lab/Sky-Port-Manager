using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.CabinClass.Commands;
using Sky.Application.UseCases.CabinClass.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/cabin-classes")]
public sealed class CabinClassesController(ISender sender) : ControllerBase
{
    public sealed record CreateCabinClassRequest(string ClassCode, string ClassName);
    public sealed record UpdateCabinClassRequest(string ClassCode, string ClassName);

    [HttpPost]
    public async Task<IActionResult> CreateCabinClass(CreateCabinClassRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCabinClassCommand(body.ClassCode, body.ClassName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{cabinClassId:int}")]
    public async Task<IActionResult> DeleteCabinClass(int cabinClassId, CancellationToken ct)
        => await sender.Send(new DeleteCabinClassCommand(cabinClassId), ct) ? NoContent() : NotFound();
    [HttpPut("{cabinClassId:int}")]
    public async Task<IActionResult> UpdateCabinClass(int cabinClassId, UpdateCabinClassRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCabinClassCommand(cabinClassId, body.ClassCode, body.ClassName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{cabinClassId:int}")]
    public async Task<IActionResult> GetCabinClassById(int cabinClassId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCabinClassByIdQuery(cabinClassId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCabinClasses(CancellationToken ct)
        => Ok(await sender.Send(new GetCabinClassesQuery(), ct));
}
