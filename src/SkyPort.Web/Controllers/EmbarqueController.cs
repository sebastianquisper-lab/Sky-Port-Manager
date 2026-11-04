using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Embarques.Queries;

namespace SkyPort.Web.Controllers;

public sealed class EmbarqueController(IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize(Policy = Policies.TodosLosRoles)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var boardings = await mediator.Send(new GetEmbarquesQuery(), cancellationToken);
        return View(boardings);
    }
}
