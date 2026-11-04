using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.UseCases.Embarques.Commands;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Embarques.Queries;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class EmbarqueController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var boardings = await mediator.Send(new GetEmbarquesQuery(), cancellationToken);
        return View(boardings);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmbarqueByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new EmbarqueViewModel();
        await PopulateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(EmbarqueViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await mediator.Send(new CreateEmbarqueCommand(model.BoletoId), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo registrar el embarque.");
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "El embarque se registró correctamente y el boleto quedó como abordado.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    private async Task PopulateOptionsAsync(EmbarqueViewModel model, CancellationToken cancellationToken)
    {
        var options = await mediator.Send(new GetEmbarqueCreateOptionsQuery(), cancellationToken);
        model.Boletos = options.Boletos;
    }
}
