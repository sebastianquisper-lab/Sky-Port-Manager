using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Empleados.Queries;
using SkyPort.Application.UseCases.Empleados.Commands;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.GestionOperaciones)]
public sealed class PersonalController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetEmpleadosQuery(), cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmpleadoByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public IActionResult Create()
    {
        var model = new EmpleadoViewModel();
        PopulateCargoOptions(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Create(EmpleadoViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new CreateEmpleadoCommand(
                model.NumeroDocumento, model.Nombres, model.Apellidos, model.Cargo, model.Email, model.Activo), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id = result.Value });
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear el empleado.");
        }

        PopulateCargoOptions(model);
        return View(model);
    }

    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmpleadoByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null) return NotFound();
        var employee = result.Value;
        var model = new EmpleadoViewModel
        {
            EmpleadoId = employee.EmpleadoId,
            NumeroDocumento = employee.NumeroDocumento,
            Nombres = employee.Nombres,
            Apellidos = employee.Apellidos,
            Cargo = employee.Cargo,
            Email = employee.Email,
            Activo = employee.Activo
        };
        PopulateCargoOptions(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Edit(int id, EmpleadoViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.EmpleadoId) return BadRequest();
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new UpdateEmpleadoCommand(
                id, model.NumeroDocumento, model.Nombres, model.Apellidos, model.Cargo, model.Email, model.Activo), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { id });
            if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo actualizar el empleado.");
        }

        PopulateCargoOptions(model);
        return View(model);
    }

    [HttpGet, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmpleadoByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Policy = Policies.SoloAdmin)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteEmpleadoCommand(id), cancellationToken);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        if (result.Error?.Type == SkyPort.Application.Common.ErrorType.NotFound) return NotFound();
        ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo eliminar el empleado.");
        var employee = await mediator.Send(new GetEmpleadoByIdQuery(id), cancellationToken);
        return employee.IsSuccess && employee.Value is not null ? View("Delete", employee.Value) : NotFound();
    }

    private static void PopulateCargoOptions(EmpleadoViewModel model) =>
        model.Cargos = new[]
        {
            new SelectListItem("Piloto", Cargo.Piloto),
            new SelectListItem("Copiloto", Cargo.Copiloto),
            new SelectListItem("Tripulación de cabina", Cargo.TripulanteCabina),
            new SelectListItem("Personal de tierra", Cargo.PersonalTierra),
            new SelectListItem("Administrativo", Cargo.Administrativo)
        };
}
