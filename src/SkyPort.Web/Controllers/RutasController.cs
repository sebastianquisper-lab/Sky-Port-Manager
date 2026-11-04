using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Rutas.Queries;

namespace SkyPort.Web.Controllers;

public sealed class RutasController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var routes = await mediator.Send(new GetRutasQuery(), cancellationToken);
        return View(routes);
    }
}
