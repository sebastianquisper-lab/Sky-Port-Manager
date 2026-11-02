using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Vuelos.Commands;
using SkyPort.Application.UseCases.Vuelos.Dtos;
using SkyPort.Application.UseCases.Vuelos.Queries;
using SkyPort.Application.UseCases.Vuelos.Rules;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class VuelosController(IMediator mediator) : Controller
{
    private const int DefaultPageSize = 10;

    private static readonly string[] FlightStates =
    [
        EstadoVuelo.Programado,
        EstadoVuelo.Embarcando,
        EstadoVuelo.Retrasado,
        EstadoVuelo.Despegado,
        EstadoVuelo.Cancelado
    ];

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(
        DateTime? desde,
        DateTime? hasta,
        string? estado,
        int? rutaId,
        int? aerolineaId,
        string? numeroVuelo,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var model = new VuelosIndexViewModel
        {
            Desde = desde,
            Hasta = hasta,
            Estado = estado,
            RutaId = rutaId,
            AerolineaId = aerolineaId,
            NumeroVuelo = numeroVuelo,
            Page = page,
            PageSize = DefaultPageSize
        };

        var lookupResult = await mediator.Send(new GetVueloLookupsQuery(), cancellationToken);
        if (lookupResult.IsSuccess)
        {
            model.Rutas = lookupResult.Value!.Rutas;
            model.Aerolineas = lookupResult.Value.Aerolineas;
        }
        else
        {
            ModelState.AddModelError(string.Empty, lookupResult.Error!.Message);
        }

        var hastaInclusive = hasta?.Date.AddDays(1).AddTicks(-1);
        var result = await mediator.Send(new GetVuelosQuery(
            new VueloFilter(desde?.Date, hastaInclusive, estado, rutaId, aerolineaId, numeroVuelo),
            page,
            DefaultPageSize), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
        }
        else
        {
            model.Vuelos = result.Value!.Items;
            model.Page = result.Value.Page;
            model.PageSize = result.Value.PageSize;
            model.Total = result.Value.Total;
        }

        ViewData["Page"] = model.Page;
        ViewData["PageSize"] = model.PageSize;
        ViewData["Total"] = model.Total;
        ViewData["RouteValues"] = new Dictionary<string, string>
        {
            ["desde"] = desde?.ToString("yyyy-MM-dd") ?? string.Empty,
            ["hasta"] = hasta?.ToString("yyyy-MM-dd") ?? string.Empty,
            ["estado"] = estado ?? string.Empty,
            ["rutaId"] = rutaId?.ToString() ?? string.Empty,
            ["aerolineaId"] = aerolineaId?.ToString() ?? string.Empty,
            ["numeroVuelo"] = numeroVuelo ?? string.Empty
        };

        return View(model);
    }

    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await LoadFlightViewModelAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new VueloViewModel();
        await LoadLookupsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VueloViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await mediator.Send(new CreateVueloCommand(
            model.NumeroVuelo,
            model.RutaId,
            model.AeronaveId,
            model.PuertaId,
            model.FechaSalida,
            model.FechaLlegada), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "El vuelo se creó correctamente.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await LoadFlightViewModelAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        await LoadLookupsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VueloViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.VueloId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await mediator.Send(new UpdateVueloCommand(
            model.VueloId,
            model.NumeroVuelo,
            model.RutaId,
            model.AeronaveId,
            model.PuertaId,
            model.FechaSalida,
            model.FechaLlegada), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "El vuelo se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id = model.VueloId });
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var model = await LoadFlightViewModelAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteVueloCommand(id), cancellationToken);
        if (!result.IsSuccess)
        {
            var model = await LoadFlightViewModelAsync(id, cancellationToken);
            if (model is null)
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return View("Delete", model);
        }

        TempData["Success"] = "El vuelo se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = Policies.GestionOperaciones)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeVueloEstadoCommand(id, nuevoEstado), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "El estado del vuelo se actualizó correctamente."
            : result.Error!.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<VueloViewModel?> LoadFlightViewModelAsync(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetVueloByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error!.Type != ErrorType.NotFound)
            {
                TempData["Error"] = result.Error.Message;
            }

            return null;
        }

        var flight = result.Value!;
        var model = new VueloViewModel
        {
            VueloId = flight.VueloId,
            NumeroVuelo = flight.NumeroVuelo,
            RutaId = flight.RutaId,
            AeronaveId = flight.AeronaveId,
            PuertaId = flight.PuertaId,
            FechaSalida = flight.FechaSalida,
            FechaLlegada = flight.FechaLlegada,
            Detalle = flight,
            EstadosSiguientes = FlightStates
                .Where(nextState => TransicionesEstadoVuelo.EsValida(flight.Estado, nextState))
                .ToArray()
        };

        return model;
    }

    private async Task LoadLookupsAsync(VueloViewModel model, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetVueloLookupsQuery(), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return;
        }

        model.Rutas = result.Value!.Rutas;
        model.Aeronaves = result.Value.Aeronaves;
        model.Puertas = result.Value.Puertas;
    }
}
