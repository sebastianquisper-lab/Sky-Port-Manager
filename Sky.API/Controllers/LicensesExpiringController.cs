using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.VLicensesExpiring.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/licenses-expiring")]
public sealed class LicensesExpiringController(ISender sender) : ControllerBase
{


    [HttpGet]
    public async Task<IActionResult> GetLicensesExpiring(CancellationToken ct)
        => Ok(await sender.Send(new GetLicensesExpiringQuery(), ct));
}
