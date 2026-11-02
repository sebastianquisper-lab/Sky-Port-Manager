using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Usuarios.Commands;
using SkyPort.Application.UseCases.Usuarios.Queries;
using SkyPort.Web.Models.Users;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.SoloAdmin)]
public sealed class UsuariosController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await mediator.Send(new ListUsersQuery(), cancellationToken);
        var model = users.Select(user => new UsuarioViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            NombreCompleto = user.NombreCompleto,
            Role = user.Role,
            Activo = user.Activo
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new CrearUsuarioViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearUsuarioViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await mediator.Send(
            new CreateUserCommand(model.Email, model.NombreCompleto, model.Password, model.Role),
            cancellationToken);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return View(model);
        }

        TempData["Success"] = "El usuario se creó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarRol(UsuarioViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Selecciona un rol válido.";
            return RedirectToAction(nameof(Index));
        }

        var result = await mediator.Send(new ChangeUserRoleCommand(model.UserId, model.Role), cancellationToken);
        SetFeedback(result, "El rol del usuario se actualizó correctamente.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(string userId, bool activo, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            TempData["Error"] = "No se pudo identificar al usuario.";
            return RedirectToAction(nameof(Index));
        }

        var result = await mediator.Send(new SetUserActiveCommand(userId, activo), cancellationToken);
        SetFeedback(result, activo ? "El usuario se activó correctamente." : "El usuario se desactivó correctamente.");
        return RedirectToAction(nameof(Index));
    }

    private void SetFeedback(SkyPort.Application.Common.Result result, string successMessage)
    {
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? successMessage : result.Error!.Message;
    }
}
