using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Roles.Queries;
using SkyPort.Application.UseCases.Roles.Commands;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.SoloAdmin)]
public sealed class RolesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var roles = await mediator.Send(new GetRolesQuery(), cancellationToken);
        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRolByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create() => View(new RolViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new CreateRolCommand(model.Name), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear el rol.");
            return View(model);
        }

        TempData["Success"] = "El rol se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRolByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(new RolViewModel { Id = result.Value.Id, Name = result.Value.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, RolViewModel model, CancellationToken cancellationToken)
    {
        if (!string.Equals(id, model.Id, StringComparison.Ordinal))
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new UpdateRolCommand(model.Id, model.Name), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar el rol.");
            return View(model);
        }

        TempData["Success"] = "El rol se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}
