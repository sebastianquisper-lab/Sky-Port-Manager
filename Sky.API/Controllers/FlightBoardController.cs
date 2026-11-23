using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.VFlightBoard.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/flight-board")]
public sealed class FlightBoardController(ISender sender) : ControllerBase
{


    [HttpGet]
    public async Task<IActionResult> GetFlightBoard(CancellationToken ct)
        => Ok(await sender.Send(new GetFlightBoardQuery(), ct));
}
