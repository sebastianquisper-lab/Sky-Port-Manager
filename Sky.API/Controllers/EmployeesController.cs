using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.Employee.Commands;
using Sky.Application.UseCases.Employee.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public sealed class EmployeesController(ISender sender) : ControllerBase
{
    public sealed record CreateEmployeeRequest(int AirportId, int DocumentTypeId, int EmployeeRoleId, string DocumentNumber, string FirstName, string LastName, string Email, string? Phone, DateOnly HireDate);
    public sealed record UpdateEmployeeRequest(int AirportId, int DocumentTypeId, int EmployeeRoleId, string DocumentNumber, string FirstName, string LastName, string Email, string? Phone, DateOnly HireDate, bool IsActive);

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateEmployeeCommand(body.AirportId, body.DocumentTypeId, body.EmployeeRoleId, body.DocumentNumber, body.FirstName, body.LastName, body.Email, body.Phone, body.HireDate), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{employeeId:int}")]
    public async Task<IActionResult> DeleteEmployee(int employeeId, CancellationToken ct)
        => await sender.Send(new DeleteEmployeeCommand(employeeId), ct) ? NoContent() : NotFound();
    [HttpPut("{employeeId:int}")]
    public async Task<IActionResult> UpdateEmployee(int employeeId, UpdateEmployeeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateEmployeeCommand(employeeId, body.AirportId, body.DocumentTypeId, body.EmployeeRoleId, body.DocumentNumber, body.FirstName, body.LastName, body.Email, body.Phone, body.HireDate, body.IsActive), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{employeeId:int}")]
    public async Task<IActionResult> GetEmployeeById(int employeeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetEmployeeByIdQuery(employeeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetEmployees(CancellationToken ct)
        => Ok(await sender.Send(new GetEmployeesQuery(), ct));
}
