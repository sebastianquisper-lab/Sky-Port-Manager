using MediatR;
using Sky.Domain.Ports;
using DocumentTypeEntity = Sky.Domain.Entities.DocumentType;

namespace Sky.Application.UseCases.DocumentType.Queries;

public sealed record GetDocumentTypeByIdQuery(
    int DocumentTypeId) : IRequest<DocumentTypeEntity?>;

internal sealed class GetDocumentTypeByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeEntity?>
{
    public Task<DocumentTypeEntity?> Handle(GetDocumentTypeByIdQuery request, CancellationToken ct)
        => uow.Repository<DocumentTypeEntity>().GetByIdAsync(request.DocumentTypeId, ct);
}
