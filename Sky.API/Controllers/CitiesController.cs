using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.City.Commands;
using Sky.Application.UseCases.City.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/cities")]
public sealed class CitiesController(ISender sender) : ControllerBase
{
    public sealed record CreateCityRequest(int CountryId, string CityName);
    public sealed record UpdateCityRequest(int CountryId, string CityName);

    [HttpPost]
    public async Task<IActionResult> CreateCity(CreateCityRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCityCommand(body.CountryId, body.CityName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{cityId:int}")]
    public async Task<IActionResult> DeleteCity(int cityId, CancellationToken ct)
        => await sender.Send(new DeleteCityCommand(cityId), ct) ? NoContent() : NotFound();
    [HttpPut("{cityId:int}")]
    public async Task<IActionResult> UpdateCity(int cityId, UpdateCityRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCityCommand(cityId, body.CountryId, body.CityName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCities(CancellationToken ct)
        => Ok(await sender.Send(new GetCitiesQuery(), ct));
    [HttpGet("{cityId:int}")]
    public async Task<IActionResult> GetCityById(int cityId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCityByIdQuery(cityId), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
