using MediatR;
using Sky.Domain.Ports;
using DocumentTypeEntity = Sky.Domain.Entities.DocumentType;

namespace Sky.Application.UseCases.DocumentType.Queries;

public sealed record GetDocumentTypesQuery : IRequest<IReadOnlyList<DocumentTypeEntity>>;

internal sealed class GetDocumentTypesHandler(IUnitOfWork uow)
    : IRequestHandler<GetDocumentTypesQuery, IReadOnlyList<DocumentTypeEntity>>
{
    public Task<IReadOnlyList<DocumentTypeEntity>> Handle(GetDocumentTypesQuery request, CancellationToken ct)
        => uow.Repository<DocumentTypeEntity>().GetAllAsync(ct);
}
