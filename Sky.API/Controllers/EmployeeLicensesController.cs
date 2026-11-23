using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.EmployeeLicense.Commands;
using Sky.Application.UseCases.EmployeeLicense.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/employee-licenses")]
public sealed class EmployeeLicensesController(ISender sender) : ControllerBase
{
    public sealed record CreateEmployeeLicenseRequest(int EmployeeId, int IssuingCountryId, string LicenseType, string LicenseNumber, DateOnly IssueDate, DateOnly ExpiryDate);
    public sealed record UpdateEmployeeLicenseRequest(int EmployeeId, int IssuingCountryId, string LicenseType, string LicenseNumber, DateOnly IssueDate, DateOnly ExpiryDate);

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeLicense(CreateEmployeeLicenseRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateEmployeeLicenseCommand(body.EmployeeId, body.IssuingCountryId, body.LicenseType, body.LicenseNumber, body.IssueDate, body.ExpiryDate), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{licenseId:int}")]
    public async Task<IActionResult> DeleteEmployeeLicense(int licenseId, CancellationToken ct)
        => await sender.Send(new DeleteEmployeeLicenseCommand(licenseId), ct) ? NoContent() : NotFound();
    [HttpPut("{licenseId:int}")]
    public async Task<IActionResult> UpdateEmployeeLicense(int licenseId, UpdateEmployeeLicenseRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateEmployeeLicenseCommand(licenseId, body.EmployeeId, body.IssuingCountryId, body.LicenseType, body.LicenseNumber, body.IssueDate, body.ExpiryDate), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{licenseId:int}")]
    public async Task<IActionResult> GetEmployeeLicenseById(int licenseId, CancellationToken ct)
    {
        var result = await sender.Send(new GetEmployeeLicenseByIdQuery(licenseId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetEmployeeLicenses(CancellationToken ct)
        => Ok(await sender.Send(new GetEmployeeLicensesQuery(), ct));
}
