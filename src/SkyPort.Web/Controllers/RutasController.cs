using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Rutas.Commands;
using SkyPort.Application.UseCases.Rutas.Queries;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class RutasController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var routes = await mediator.Send(new GetRutasQuery(), cancellationToken);
        return View(routes);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public IActionResult Create() => View(new RutaViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Create(RutaViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new CreateRutaCommand(
            model.CodigoOrigen,
            model.CiudadOrigen,
            model.CodigoDestino,
            model.CiudadDestino,
            model.DuracionMinutos), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear la ruta.");
            return View(model);
        }

        TempData["Success"] = "La ruta se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }
}
