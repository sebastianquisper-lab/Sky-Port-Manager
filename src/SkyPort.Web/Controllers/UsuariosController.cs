using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Usuarios.Commands;
using SkyPort.Application.UseCases.Usuarios.Queries;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.SoloAdmin)]
public sealed class UsuariosController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var usuarios = await mediator.Send(new GetUsuariosQuery(page, pageSize), cancellationToken);
        ViewData["Page"] = usuarios.PageNumber;
        ViewData["PageSize"] = usuarios.PageSize;
        ViewData["Total"] = usuarios.TotalCount;
        return View(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsuarioByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create() => View(new Models.CrearUsuarioViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Models.CrearUsuarioViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var result = await mediator.Send(new CreateUsuarioCommand(model.Email, model.Password), cancellationToken);
        model.Password = string.Empty;
        model.ConfirmPassword = string.Empty;
        if (!result.IsSuccess || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear el usuario.");
            return View(model);
        }

        TempData["Success"] = "El usuario se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }
}
