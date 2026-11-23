using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Passenger.Commands;
using Sky.Application.UseCases.Passenger.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/passengers")]
public sealed class PassengersController(ISender sender) : ControllerBase
{
    public sealed record CreatePassengerRequest(int DocumentTypeId, string DocumentNumber, string FirstName, string LastName, DateOnly? BirthDate, int? NationalityCountryId, string? Email, string? Phone);
    public sealed record UpdatePassengerRequest(int DocumentTypeId, string DocumentNumber, string FirstName, string LastName, DateOnly? BirthDate, int? NationalityCountryId, string? Email, string? Phone);

    [HttpPost]
    public async Task<IActionResult> CreatePassenger(CreatePassengerRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreatePassengerCommand(body.DocumentTypeId, body.DocumentNumber, body.FirstName, body.LastName, body.BirthDate, body.NationalityCountryId, body.Email, body.Phone), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{passengerId:int}")]
    public async Task<IActionResult> DeletePassenger(int passengerId, CancellationToken ct)
        => await sender.Send(new DeletePassengerCommand(passengerId), ct) ? NoContent() : NotFound();
    [HttpPut("{passengerId:int}")]
    public async Task<IActionResult> UpdatePassenger(int passengerId, UpdatePassengerRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdatePassengerCommand(passengerId, body.DocumentTypeId, body.DocumentNumber, body.FirstName, body.LastName, body.BirthDate, body.NationalityCountryId, body.Email, body.Phone), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{passengerId:int}")]
    public async Task<IActionResult> GetPassengerById(int passengerId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPassengerByIdQuery(passengerId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetPassengers(CancellationToken ct)
        => Ok(await sender.Send(new GetPassengersQuery(), ct));
}
