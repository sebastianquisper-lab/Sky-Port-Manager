using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Country.Commands;
using Sky.Application.UseCases.Country.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/countries")]
public sealed class CountriesController(ISender sender) : ControllerBase
{
    public sealed record CreateCountryRequest(string CountryName, string Iso2Code, string Iso3Code);
    public sealed record UpdateCountryRequest(string CountryName, string Iso2Code, string Iso3Code);

    [HttpPost]
    public async Task<IActionResult> CreateCountry(CreateCountryRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCountryCommand(body.CountryName, body.Iso2Code, body.Iso3Code), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{countryId:int}")]
    public async Task<IActionResult> DeleteCountry(int countryId, CancellationToken ct)
        => await sender.Send(new DeleteCountryCommand(countryId), ct) ? NoContent() : NotFound();
    [HttpPut("{countryId:int}")]
    public async Task<IActionResult> UpdateCountry(int countryId, UpdateCountryRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCountryCommand(countryId, body.CountryName, body.Iso2Code, body.Iso3Code), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCountries(CancellationToken ct)
        => Ok(await sender.Send(new GetCountriesQuery(), ct));
    [HttpGet("{countryId:int}")]
    public async Task<IActionResult> GetCountryById(int countryId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCountryByIdQuery(countryId), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
