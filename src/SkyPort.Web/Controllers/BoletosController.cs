using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Boletos.Queries;
using SkyPort.Application.UseCases.Boletos.Commands;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Controllers;

public sealed class BoletosController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(int? reservaId, CancellationToken cancellationToken)
    {
        var model = new BoletoViewModel { ReservaId = reservaId.GetValueOrDefault() };
        await PopulateReservationsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(BoletoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateReservationsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await mediator.Send(new CreateBoletoCommand(model.ReservaId), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo emitir el boleto.");
            await PopulateReservationsAsync(model, cancellationToken);
            return View(model);
        }
        TempData["Success"] = "El boleto se emitió correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

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

    private async Task PopulateReservationsAsync(BoletoViewModel model, CancellationToken cancellationToken)
    {
        var options = await mediator.Send(new GetReservasParaBoletoQuery(), cancellationToken);
        model.Reservas = options.Select(x => new SelectListItem($"{x.Codigo} · {x.NombrePasajero} · {x.NumeroVuelo} · {x.Asiento}", x.ReservaId.ToString())).ToList();
    }
}
