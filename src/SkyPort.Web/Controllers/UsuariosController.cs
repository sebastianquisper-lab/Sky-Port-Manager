using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        var memberships = await mediator.Send(new GetUsuarioRolesQuery(id), cancellationToken);
        if (!memberships.IsSuccess || memberships.Value is null)
        {
            return NotFound();
        }

        return View(new Models.UsuarioAdminDetailsViewModel(result.Value, memberships.Value));
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

    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsuarioByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(new Models.UsuarioPerfilViewModel { Id = result.Value.Id, Email = result.Value.Email ?? string.Empty, UserName = result.Value.UserName ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Models.UsuarioPerfilViewModel model, CancellationToken cancellationToken)
    {
        if (!string.Equals(id, model.Id, StringComparison.Ordinal))
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(new UpdateUsuarioCommand(model.Id, model.Email, model.UserName), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar el perfil.");
            return View(model);
        }

        TempData["Success"] = "El perfil se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string roleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AsignarUsuarioRolCommand(userId, roleId), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "El rol se asignó correctamente. La cookie de la cuenta se actualizará en su próxima validación de seguridad."
            : result.Error?.Message ?? "No se pudo asignar el rol.";
        return RedirectToAction(nameof(Details), new { id = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeRole(string userId, string roleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevocarUsuarioRolCommand(userId, roleId), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "El rol se revocó correctamente. La cookie de la cuenta se actualizará en su próxima validación de seguridad."
            : result.Error?.Message ?? "No se pudo revocar el rol.";
        return RedirectToAction(nameof(Details), new { id = userId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsuarioByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return NotFound();
        }

        return View(new Models.DeleteUsuarioViewModel(result.Value.Id, result.Value.Email));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken cancellationToken)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(actorId))
        {
            return Forbid();
        }

        var result = await mediator.Send(new DeleteUsuarioCommand(id, actorId), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error?.Message ?? "No se pudo eliminar el usuario.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "El usuario se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
