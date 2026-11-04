using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Equipajes.Queries;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.TodosLosRoles)]
public sealed class EquipajeController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await mediator.Send(new GetEquipajesQuery(), cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEquipajeByIdQuery(id), cancellationToken);
        return result.IsSuccess && result.Value is not null ? View(result.Value) : NotFound();
    }
}
