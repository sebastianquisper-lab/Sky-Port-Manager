using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Aeronaves.Queries;
namespace SkyPort.Web.Controllers;
public sealed class AeronavesController(IMediator mediator) : Controller
{
    [HttpGet, Authorize(Policy = Policies.GestionOperaciones)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetAeronavesQuery(), cancellationToken));
}
