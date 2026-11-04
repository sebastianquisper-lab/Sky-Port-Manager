using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Boletos.Queries;

namespace SkyPort.Web.Controllers;

public sealed class BoletosController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var boletos = await mediator.Send(new GetBoletosQuery(page, pageSize), cancellationToken);
        ViewData["Page"] = boletos.PageNumber;
        ViewData["PageSize"] = boletos.PageSize;
        ViewData["Total"] = boletos.TotalCount;
        ViewData["RouteValues"] = new Dictionary<string, string>();
        return View(boletos);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBoletoByIdQuery(id), cancellationToken);
        return !result.IsSuccess || result.Value is null ? NotFound() : View(result.Value);
    }
}
