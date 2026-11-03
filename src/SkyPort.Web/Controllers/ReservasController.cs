using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Pasajeros;
using SkyPort.Application.UseCases.Reservas;
using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Web.Models;

namespace SkyPort.Web.Controllers;

public sealed class ReservasController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vuelos = await mediator.Send(new GetVuelosAbiertosQuery(), cancellationToken);
        return View(vuelos.Where(vuelo => vuelo.AsientosLibres > 0).ToList());
    }

    [HttpGet("/Reservas/Vuelo/{vueloId:int}")]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Vuelo(int vueloId, CancellationToken cancellationToken)
    {
        var reservas = await mediator.Send(new GetReservasByVueloQuery(vueloId), cancellationToken);
        ViewData["VueloId"] = vueloId;
        return View(reservas);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(
        int vueloId,
        string? tipoDocumento,
        string? numeroDocumento,
        CancellationToken cancellationToken)
    {
        var model = await BuildCreateViewModelAsync(vueloId, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        model.TipoDocumento = tipoDocumento;
        model.NumeroDocumento = numeroDocumento;
        if (!string.IsNullOrWhiteSpace(tipoDocumento) || !string.IsNullOrWhiteSpace(numeroDocumento))
        {
            if (string.IsNullOrWhiteSpace(tipoDocumento) || string.IsNullOrWhiteSpace(numeroDocumento))
            {
                ModelState.AddModelError(string.Empty, "Ingresa el tipo y el número de documento.");
            }
            else
            {
                var result = await mediator.Send(
                    new GetPasajeroByDocumentoQuery(tipoDocumento, numeroDocumento),
                    cancellationToken);
                if (result.IsSuccess && result.Value is not null)
                {
                    model.PasajeroId = result.Value.PasajeroId;
                    model.NombrePasajero = $"{result.Value.Nombres} {result.Value.Apellidos}";
                    model.TipoDocumento = result.Value.TipoDocumento;
                    model.NumeroDocumento = result.Value.NumeroDocumento;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se encontró el pasajero.");
                    model.PasajeroNoEncontrado = result.Error?.Type == ErrorType.NotFound;
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Create(CreateReservaViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.PasajeroId is null || string.IsNullOrWhiteSpace(model.Asiento))
        {
            ModelState.AddModelError(string.Empty, "Busca un pasajero y selecciona un asiento disponible.");
            return await ReturnCreateViewAsync(model, cancellationToken);
        }

        var result = await mediator.Send(
            new CreateReservaCommand(model.PasajeroId.Value, model.VueloId, model.Asiento),
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error?.Message ?? "No se pudo crear la reserva.");
            return await ReturnCreateViewAsync(model, cancellationToken);
        }

        TempData["Success"] = $"La reserva {result.Value.Codigo} se creó correctamente.";
        return RedirectToAction(nameof(Vuelo), new { vueloId = model.VueloId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.GestionPasajeros)]
    public async Task<IActionResult> Cancelar(int reservaId, int vueloId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelReservaCommand(reservaId), cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error?.Message ?? "No se pudo cancelar la reserva.";
        }
        else
        {
            TempData["Success"] = "La reserva se canceló correctamente.";
        }

        return RedirectToAction(nameof(Vuelo), new { vueloId });
    }

    private async Task<IActionResult> ReturnCreateViewAsync(
        CreateReservaViewModel model,
        CancellationToken cancellationToken)
    {
        var currentModel = await BuildCreateViewModelAsync(model.VueloId, cancellationToken);
        if (currentModel is null)
        {
            return NotFound();
        }

        currentModel.PasajeroId = model.PasajeroId;
        currentModel.NombrePasajero = model.NombrePasajero;
        currentModel.PasajeroNoEncontrado = model.PasajeroNoEncontrado;
        currentModel.TipoDocumento = model.TipoDocumento;
        currentModel.NumeroDocumento = model.NumeroDocumento;
        currentModel.Asiento = model.Asiento;
        return View("Create", currentModel);
    }

    private async Task<CreateReservaViewModel?> BuildCreateViewModelAsync(
        int vueloId,
        CancellationToken cancellationToken)
    {
        var vuelos = await mediator.Send(new GetVuelosAbiertosQuery(), cancellationToken);
        var vuelo = vuelos.FirstOrDefault(item => item.VueloId == vueloId);
        if (vuelo is null)
        {
            return null;
        }

        var result = await mediator.Send(new GetAsientosVueloQuery(vueloId), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return null;
        }

        return new CreateReservaViewModel
        {
            VueloId = vuelo.VueloId,
            NumeroVuelo = vuelo.NumeroVuelo,
            Ruta = $"{vuelo.CodigoOrigen} - {vuelo.CodigoDestino}",
            FechaSalida = vuelo.FechaSalida,
            EstadoVuelo = vuelo.Estado,
            Capacidad = result.Value.Capacidad,
            ReservasConfirmadas = vuelo.ReservasConfirmadas,
            AsientosLibres = vuelo.AsientosLibres,
            AsientosOcupados = result.Value.AsientosOcupados
        };
    }
}
