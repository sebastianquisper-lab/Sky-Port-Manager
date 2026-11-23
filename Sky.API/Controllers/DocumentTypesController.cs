using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.DocumentType.Commands;
using Sky.Application.UseCases.DocumentType.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/document-types")]
public sealed class DocumentTypesController(ISender sender) : ControllerBase
{
    public sealed record CreateDocumentTypeRequest(string TypeName, string Abbreviation);
    public sealed record UpdateDocumentTypeRequest(string TypeName, string Abbreviation);

    [HttpPost]
    public async Task<IActionResult> CreateDocumentType(CreateDocumentTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreateDocumentTypeCommand(body.TypeName, body.Abbreviation), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{documentTypeId:int}")]
    public async Task<IActionResult> DeleteDocumentType(int documentTypeId, CancellationToken ct)
        => await sender.Send(new DeleteDocumentTypeCommand(documentTypeId), ct) ? NoContent() : NotFound();
    [HttpPut("{documentTypeId:int}")]
    public async Task<IActionResult> UpdateDocumentType(int documentTypeId, UpdateDocumentTypeRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateDocumentTypeCommand(documentTypeId, body.TypeName, body.Abbreviation), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{documentTypeId:int}")]
    public async Task<IActionResult> GetDocumentTypeById(int documentTypeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDocumentTypeByIdQuery(documentTypeId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetDocumentTypes(CancellationToken ct)
        => Ok(await sender.Send(new GetDocumentTypesQuery(), ct));
}
