using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Rutas.Commands;
using SkyPort.Application.UseCases.Rutas.Dtos;
using SkyPort.Application.UseCases.Rutas.Queries;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class RutasController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(string? texto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutasQuery(texto), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error!.Message;
            return View(Array.Empty<RutaDto>());
        }

        ViewData["Texto"] = texto;
        return View(result.Value!);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(ToViewModel(result.Value!));
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public IActionResult Create() => View(new RutaViewModel());

    [HttpPost]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
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
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return View(model);
        }

        TempData["Success"] = "La ruta se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        return result.IsSuccess ? View(ToViewModel(result.Value!)) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RutaViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.RutaId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new UpdateRutaCommand(
            model.RutaId,
            model.CodigoOrigen,
            model.CiudadOrigen,
            model.CodigoDestino,
            model.CiudadDestino,
            model.DuracionMinutos), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return View(model);
        }

        TempData["Success"] = "La ruta se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.RutaId });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        return result.IsSuccess ? View(ToViewModel(result.Value!)) : NotFound();
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteRutaCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            var routeResult = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
            if (!routeResult.IsSuccess)
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return View("Delete", ToViewModel(routeResult.Value!));
        }

        TempData["Success"] = "La ruta se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static RutaViewModel ToViewModel(RutaDto route) => new()
    {
        RutaId = route.RutaId,
        CodigoOrigen = route.CodigoOrigen,
        CiudadOrigen = route.CiudadOrigen,
        CodigoDestino = route.CodigoDestino,
        CiudadDestino = route.CiudadDestino,
        DuracionMinutos = route.DuracionMinutos
    };
}
