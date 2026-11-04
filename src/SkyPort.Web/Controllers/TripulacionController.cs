using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.TripulacionVuelo.Queries;
using SkyPort.Application.UseCases.TripulacionVuelo.Commands;
using SkyPort.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SkyPort.Web.Controllers;
[Authorize(Policy = Policies.GestionOperaciones)]
public sealed class TripulacionController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new TripulacionViewModel();
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TripulacionViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new CreateTripulacionVueloCommand(model.VueloId, model.EmpleadoId, model.RolAbordo), cancellationToken);
            if (result.IsSuccess) return RedirectToAction(nameof(Details), new { vueloId = model.VueloId, empleadoId = model.EmpleadoId });
            ModelState.AddModelError(string.Empty, result.Error!.Message);
        }
        await PopulateAsync(model, cancellationToken);
        return View(model);
    }

    private async Task PopulateAsync(TripulacionViewModel model, CancellationToken cancellationToken)
    {
        var options = await mediator.Send(new GetTripulacionOptionsQuery(), cancellationToken);
        model.Vuelos = options.Vuelos.Select(x => new SelectListItem(x.Label, x.Id.ToString())).ToList();
        model.Empleados = options.Empleados.Select(x => new SelectListItem(x.Label, x.Id.ToString())).ToList();
    }
    [HttpGet]
    public async Task<IActionResult> Details(int vueloId, int empleadoId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTripulacionVueloByIdQuery(vueloId, empleadoId), cancellationToken);
        return result.IsSuccess ? View(result.Value) : NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetTripulacionesVueloQuery(), cancellationToken));
}
