using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.TripulacionVuelo.Queries;
namespace SkyPort.Web.Controllers;
[Authorize(Policy = Policies.GestionOperaciones)]
public sealed class TripulacionController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetTripulacionesVueloQuery(), cancellationToken));
}
