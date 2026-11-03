using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Pasajeros;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class PasajerosController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(
        string? busqueda,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pasajeros = await mediator.Send(new GetPasajerosQuery(busqueda, page, pageSize), cancellationToken);
        ViewData["Page"] = pasajeros.PageNumber;
        ViewData["PageSize"] = pasajeros.PageSize;
        ViewData["Total"] = pasajeros.TotalCount;
        ViewData["RouteValues"] = string.IsNullOrWhiteSpace(busqueda)
            ? new Dictionary<string, string>()
            : new Dictionary<string, string> { ["busqueda"] = busqueda };

        return View(pasajeros);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPasajeroByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(PasajeroViewModel.FromDto(result.Value));
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public IActionResult Create() => View(new PasajeroViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(PasajeroViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new CreatePasajeroCommand(
            model.TipoDocumento,
            model.NumeroDocumento,
            model.Nombres,
            model.Apellidos,
            model.FechaNacimiento,
            model.Nacionalidad,
            model.Email,
            model.Telefono), cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear el pasajero.");
            return View(model);
        }

        TempData["Success"] = "El pasajero se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value.PasajeroId });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPasajeroByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(PasajeroViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Edit(int id, PasajeroViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.PasajeroId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new UpdatePasajeroCommand(
            model.PasajeroId,
            model.TipoDocumento,
            model.NumeroDocumento,
            model.Nombres,
            model.Apellidos,
            model.FechaNacimiento,
            model.Nacionalidad,
            model.Email,
            model.Telefono), cancellationToken);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar el pasajero.");
            return View(model);
        }

        TempData["Success"] = "Los datos del pasajero se actualizaron correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.PasajeroId });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPasajeroByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(PasajeroViewModel.FromDto(result.Value));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeletePasajeroCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error?.Message ?? "No se pudo eliminar el pasajero.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "El pasajero se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
