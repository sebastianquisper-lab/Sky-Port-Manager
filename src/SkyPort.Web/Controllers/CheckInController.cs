using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.CheckIn.Queries;

namespace SkyPort.Web.Controllers;

public sealed class CheckInController(IMediator mediator) : Controller
{
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
}
