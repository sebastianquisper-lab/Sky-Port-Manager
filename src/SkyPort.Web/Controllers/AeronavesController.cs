using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Aeronaves.Queries;
using SkyPort.Application.UseCases.Aeronaves.Commands;
using SkyPort.Application.UseCases.Aerolineas.Queries;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SkyPort.Web.Controllers;
public sealed class AeronavesController(IMediator mediator) : Controller
{
    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAeronaveByIdQuery(id), cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteAeronaveCommand(id), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();
        ModelState.AddModelError(string.Empty, result.Error!.Message);
        var row = await mediator.Send(new GetAeronaveByIdQuery(id), cancellationToken);
        return row.IsSuccess ? View("Delete", row.Value) : NotFound();
    }
    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAeronaveByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return NotFound();
        var row = result.Value;
        var model = new AeronaveViewModel { AeronaveId = row.AeronaveId, AerolineaId = row.AerolineaId, Matricula = row.Matricula, Modelo = row.Modelo, Capacidad = row.Capacidad, Estado = row.Estado };
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Edit(int id, AeronaveViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.AeronaveId) return BadRequest();
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new UpdateAeronaveCommand(id, model.AerolineaId, model.Matricula, model.Modelo, model.Capacidad, model.Estado), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id });
            if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();
            ModelState.AddModelError(string.Empty, result.Error!.Message);
        }
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }
    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new AeronaveViewModel();
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Create(AeronaveViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new CreateAeronaveCommand(model.AerolineaId, model.Matricula, model.Modelo, model.Capacidad, model.Estado), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id = result.Value });
            ModelState.AddModelError(string.Empty, result.Error!.Message);
        }
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }

    private async Task PopulateAsync(AeronaveViewModel model, CancellationToken cancellationToken)
    {
        var rows = await mediator.Send(new GetAerolineasQuery(), cancellationToken);
        model.Aerolineas = rows.Select(x => new SelectListItem(x.Nombre, x.AerolineaId.ToString())).ToList();
    }
    [HttpGet, Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAeronaveByIdQuery(id), cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }
    [HttpGet, Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetAeronavesQuery(), cancellationToken));
}
