using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.AuditLog.Commands;
using Sky.Application.UseCases.AuditLog.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/audit-logs")]
public sealed class AuditLogsController(ISender sender) : ControllerBase
{
    public sealed record CreateAuditLogRequest(string TableName, string RecordId, string Action, string? OldValues, string? NewValues);

    [HttpPost]
    public async Task<IActionResult> CreateAuditLog(CreateAuditLogRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateAuditLogCommand(body.TableName, body.RecordId, body.Action, body.OldValues, body.NewValues), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpGet("{auditId}")]
    public async Task<IActionResult> GetAuditLogById(long auditId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAuditLogByIdQuery(auditId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(CancellationToken ct)
        => Ok(await sender.Send(new GetAuditLogsQuery(), ct));
}
