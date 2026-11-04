using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Vuelos.Commands;
using SkyPort.Application.UseCases.Vuelos.Queries;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class VuelosController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var flights = await mediator.Send(new GetVuelosQuery(), cancellationToken);
        return View(flights);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetVueloByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new VueloCreateViewModel();
        await PopulateCreateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Create(VueloCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await mediator.Send(new CreateVueloCommand(
            model.NumeroVuelo,
            model.RutaId,
            model.AeronaveId,
            model.PuertaId,
            model.FechaSalida!.Value,
            model.FechaLlegada!.Value), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear el vuelo.");
            await PopulateCreateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "El vuelo se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    private async Task PopulateCreateOptionsAsync(VueloCreateViewModel model, CancellationToken cancellationToken)
    {
        var options = await mediator.Send(new GetVueloCreateOptionsQuery(), cancellationToken);
        model.Rutas = options.Rutas.Select(route => new SelectListItem(route.Etiqueta, route.RutaId.ToString())).ToList();
        model.Aeronaves = options.Aeronaves.Select(aircraft => new SelectListItem(aircraft.Etiqueta, aircraft.AeronaveId.ToString())).ToList();
        model.Puertas = options.Puertas.Select(gate => new SelectListItem(gate.Etiqueta, gate.PuertaId.ToString())).ToList();
    }
}
