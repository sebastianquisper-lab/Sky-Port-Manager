using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.VFlightOccupancy.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-occupancy")]
public sealed class FlightOccupancyController(ISender sender) : ControllerBase
{


    [HttpGet]
    public async Task<IActionResult> GetFlightOccupancy(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightOccupancyQuery(), ct));
}
