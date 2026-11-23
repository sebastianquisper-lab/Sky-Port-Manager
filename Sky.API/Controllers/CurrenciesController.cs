using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Currency.Commands;
using Sky.Application.UseCases.Currency.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/currencies")]
public sealed class CurrenciesController(ISender sender) : ControllerBase
{
    public sealed record CreateCurrencyRequest(string CurrencyCode, string CurrencyName, string? Symbol);
    public sealed record UpdateCurrencyRequest(string CurrencyName, string? Symbol);

    [HttpPost]
    public async Task<IActionResult> CreateCurrency(CreateCurrencyRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateCurrencyCommand(body.CurrencyCode, body.CurrencyName, body.Symbol), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{currencyCode}")]
    public async Task<IActionResult> DeleteCurrency(string currencyCode, CancellationToken ct)
        => await sender.Send(new DeleteCurrencyCommand(currencyCode), ct) ? NoContent() : NotFound();
    [HttpPut("{currencyCode}")]
    public async Task<IActionResult> UpdateCurrency(string currencyCode, UpdateCurrencyRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCurrencyCommand(currencyCode, body.CurrencyName, body.Symbol), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCurrencies(CancellationToken ct)
        => Ok(await sender.Send(new GetCurrenciesQuery(), ct));
    [HttpGet("{currencyCode}")]
    public async Task<IActionResult> GetCurrencyById(string currencyCode, CancellationToken ct)
    {
        var result = await sender.Send(new GetCurrencyByIdQuery(currencyCode), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
