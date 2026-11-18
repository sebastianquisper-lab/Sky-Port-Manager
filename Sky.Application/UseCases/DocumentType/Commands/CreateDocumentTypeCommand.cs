using MediatR;
using Sky.Domain.Ports;
using DocumentTypeEntity = Sky.Domain.Entities.DocumentType;

namespace Sky.Application.UseCases.DocumentType.Commands;

public sealed record CreateDocumentTypeCommand(
    string TypeName,
    string Abbreviation) : IRequest<DocumentTypeEntity>;

internal sealed class CreateDocumentTypeHandler(IUnitOfWork uow)
    : IRequestHandler<CreateDocumentTypeCommand, DocumentTypeEntity>
{
    public async Task<DocumentTypeEntity> Handle(CreateDocumentTypeCommand request, CancellationToken ct)
    {
        var entity = new DocumentTypeEntity
        {
            TypeName = request.TypeName,
            Abbreviation = request.Abbreviation
        };

        await uow.Repository<DocumentTypeEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
