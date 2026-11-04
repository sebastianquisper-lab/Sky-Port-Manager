using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Terminales.Queries;
using SkyPort.Application.UseCases.Terminales.Commands;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class TerminalesController(IMediator mediator) : Controller
{
    private const string ReadPolicy = Policies.GestionOperaciones; // Provisional, pendiente de acuerdo específico.
    private const string WritePolicy = Policies.SoloAdmin; // Provisional, pendiente de acuerdo específico.

    // Permisos provisionales: la matriz del equipo no especifica Terminales.
    [HttpGet, Authorize(Policy = ReadPolicy)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetTerminalesQuery(), cancellationToken));

    // Permisos provisionales: la matriz del equipo no especifica Terminales.
    [HttpGet, Authorize(Policy = ReadPolicy)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTerminalByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public IActionResult Create() => View(new TerminalViewModel());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Create(TerminalViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await mediator.Send(new CreateTerminalCommand(model.Nombre), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id = result.Value });

        ModelState.AddModelError(nameof(model.Nombre), result.Error?.Message ?? "No se pudo crear la terminal.");
        return View(model);
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTerminalByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return NotFound();
        return View(new TerminalViewModel { TerminalId = result.Value.TerminalId, Nombre = result.Value.Nombre });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Edit(int id, TerminalViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.TerminalId) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var result = await mediator.Send(new UpdateTerminalCommand(id, model.Nombre), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id });
        if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();

        ModelState.AddModelError(nameof(model.Nombre), result.Error?.Message ?? "No se pudo actualizar la terminal.");
        return View(model);
    }

    [HttpGet, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTerminalByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Policy = WritePolicy)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTerminalCommand(id), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();

        ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo eliminar la terminal.");
        var terminal = await mediator.Send(new GetTerminalByIdQuery(id), cancellationToken);
        return terminal.IsSuccess && terminal.Value is not null ? View("Delete", terminal.Value) : NotFound();
    }
}
