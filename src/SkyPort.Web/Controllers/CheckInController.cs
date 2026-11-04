using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.CheckIn.Queries;
using SkyPort.Application.UseCases.CheckIn.Commands;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Controllers;

public sealed class CheckInController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CheckInViewModel();
        await PopulateSelectorsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(CheckInViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectorsAsync(model, cancellationToken);
            return View(model);
        }
        var result = await mediator.Send(new CreateCheckInCommand(model.BoletoId, model.EmpleadoId, model.DocumentoVerificado), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo registrar el check-in.");
            await PopulateSelectorsAsync(model, cancellationToken);
            return View(model);
        }
        TempData["Success"] = "El check-in se registró correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var records = await mediator.Send(new GetCheckInsQuery(page, pageSize), cancellationToken);
        ViewData["Page"] = records.PageNumber;
        ViewData["PageSize"] = records.PageSize;
        ViewData["Total"] = records.TotalCount;
        ViewData["RouteValues"] = new Dictionary<string, string>();
        return View(records);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCheckInByIdQuery(id), cancellationToken);
        return !result.IsSuccess || result.Value is null ? NotFound() : View(result.Value);
    }

    private async Task PopulateSelectorsAsync(CheckInViewModel model, CancellationToken cancellationToken)
    {
        var tickets = await mediator.Send(new GetBoletosParaCheckInQuery(), cancellationToken);
        var employees = await mediator.Send(new GetCheckInEmployeesQuery(), cancellationToken);
        model.Boletos = tickets.Select(x => new SelectListItem($"{x.CodigoBoleto} · {x.CodigoReserva} · {x.NombrePasajero}", x.BoletoId.ToString())).ToList();
        model.Empleados = employees.Select(x => new SelectListItem(x.Nombre, x.EmpleadoId.ToString())).ToList();
    }
}
