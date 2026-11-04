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

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(ToEditViewModel(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Edit(int id, EditRutaViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.RutaId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return await ReturnEditViewAsync(model, cancellationToken);
        }

        var result = await mediator.Send(new UpdateRutaCommand(model.RutaId, model.DuracionMinutos), cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound)
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar la ruta.");
            return await ReturnEditViewAsync(model, cancellationToken);
        }

        TempData["Success"] = "La duración de la ruta se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.RutaId });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRutaByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteRutaCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound)
            {
                return NotFound();
            }

            TempData["Error"] = result.Error?.Message ?? "No se pudo eliminar la ruta.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "La ruta se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ReturnEditViewAsync(EditRutaViewModel model, CancellationToken cancellationToken)
    {
        var current = await mediator.Send(new GetRutaByIdQuery(model.RutaId), cancellationToken);
        if (!current.IsSuccess || current.Value is null)
        {
            return NotFound();
        }

        var viewModel = ToEditViewModel(current.Value);
        viewModel.DuracionMinutos = model.DuracionMinutos;
        return View("Edit", viewModel);
    }

    private static EditRutaViewModel ToEditViewModel(SkyPort.Application.Dtos.Rutas.RutaDto route) => new()
    {
        RutaId = route.RutaId,
        CodigoOrigen = route.CodigoOrigen,
        CiudadOrigen = route.CiudadOrigen,
        CodigoDestino = route.CodigoDestino,
        CiudadDestino = route.CiudadDestino,
        DuracionMinutos = route.DuracionMinutos
    };
}
