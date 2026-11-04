using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Puertas.Queries;
using SkyPort.Application.Common;
using SkyPort.Application.UseCases.Puertas.Commands;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Controllers;

public sealed class PuertasController(IMediator mediator) : Controller
{
    private const string ReadPolicy = Policies.GestionOperaciones; // Provisional, pendiente de acuerdo específico.
    private const string WritePolicy = Policies.SoloAdmin; // Provisional, pendiente de acuerdo específico.

    // Permisos provisionales: la matriz del equipo no especifica Puertas.
    [HttpGet, Authorize(Policy = ReadPolicy)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetPuertasQuery(), cancellationToken));

    // Permisos provisionales: la matriz del equipo no especifica Puertas.
    [HttpGet, Authorize(Policy = ReadPolicy)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPuertaByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PuertaViewModel();
        await PopulateTerminalsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Create(PuertaViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new CreatePuertaCommand(model.TerminalId, model.Codigo, model.Disponible), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id = result.Value });
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear la puerta.");
        }

        await PopulateTerminalsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPuertaByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return NotFound();
        var model = new PuertaViewModel
        {
            PuertaId = result.Value.PuertaId,
            TerminalId = result.Value.TerminalId,
            Codigo = result.Value.Codigo,
            Disponible = result.Value.Disponible
        };
        await PopulateTerminalsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Edit(int id, PuertaViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.PuertaId) return BadRequest();
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new UpdatePuertaCommand(id, model.TerminalId, model.Codigo, model.Disponible), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id });
            if (result.Error?.Type == ErrorType.NotFound) return NotFound();
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar la puerta.");
        }

        await PopulateTerminalsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPuertaByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeletePuertaCommand(id), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        if (result.Error?.Type == ErrorType.NotFound) return NotFound();
        ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo eliminar la puerta.");
        var gate = await mediator.Send(new GetPuertaByIdQuery(id), cancellationToken);
        return gate.IsSuccess && gate.Value is not null ? View("Delete", gate.Value) : NotFound();
    }

    private async Task PopulateTerminalsAsync(PuertaViewModel model, CancellationToken cancellationToken)
    {
        var terminals = await mediator.Send(new GetTerminalChoicesQuery(), cancellationToken);
        model.Terminales = terminals.Select(x => new SelectListItem(x.Nombre, x.TerminalId.ToString())).ToList();
    }
}
