using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Aerolineas.Queries;

namespace SkyPort.Web.Controllers;

public sealed class AerolineasController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var aerolineas = await mediator.Send(new GetAerolineasQuery(), cancellationToken);
        return View(aerolineas);
    }
}
