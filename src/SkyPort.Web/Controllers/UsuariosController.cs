using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Application.UseCases.Usuarios.Queries;

namespace SkyPort.Web.Controllers;

[Authorize(Policy = Policies.SoloAdmin)]
public sealed class UsuariosController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var usuarios = await mediator.Send(new GetUsuariosQuery(page, pageSize), cancellationToken);
        ViewData["Page"] = usuarios.PageNumber;
        ViewData["PageSize"] = usuarios.PageSize;
        ViewData["Total"] = usuarios.TotalCount;
        return View(usuarios);
    }
}
