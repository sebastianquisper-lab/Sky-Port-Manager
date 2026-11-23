using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Fare.Commands;
using Sky.Application.UseCases.Fare.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/fares")]
public sealed class FaresController(ISender sender) : ControllerBase
{
    public sealed record CreateFareRequest(int AirlineId, int RouteId, int CabinClassId, string CurrencyCode, decimal BasePrice, DateOnly ValidFrom, DateOnly? ValidTo);
    public sealed record UpdateFareRequest(int AirlineId, int RouteId, int CabinClassId, string CurrencyCode, decimal BasePrice, DateOnly ValidFrom, DateOnly? ValidTo, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateFare(CreateFareRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateFareCommand(body.AirlineId, body.RouteId, body.CabinClassId, body.CurrencyCode, body.BasePrice, body.ValidFrom, body.ValidTo), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{fareId:int}")]
    public async Task<IActionResult> DeleteFare(int fareId, CancellationToken ct)
        => await sender.Send(new DeleteFareCommand(fareId), ct) ? NoContent() : NotFound();
    [HttpPut("{fareId:int}")]
    public async Task<IActionResult> UpdateFare(int fareId, UpdateFareRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateFareCommand(fareId, body.AirlineId, body.RouteId, body.CabinClassId, body.CurrencyCode, body.BasePrice, body.ValidFrom, body.ValidTo, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{fareId:int}")]
    public async Task<IActionResult> GetFareById(int fareId, CancellationToken ct)
    {
        var result = await sender.Send(new GetFareByIdQuery(fareId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFares(CancellationToken ct)
        => Ok(await sender.Send(new GetFaresQuery(), ct));
}
