using MediatR;
using Sky.Domain.Ports;
using DocumentTypeEntity = Sky.Domain.Entities.DocumentType;

namespace Sky.Application.UseCases.DocumentType.Commands;

public sealed record UpdateDocumentTypeCommand(
    int DocumentTypeId,
    string TypeName,
    string Abbreviation) : IRequest<DocumentTypeEntity?>;

internal sealed class UpdateDocumentTypeHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateDocumentTypeCommand, DocumentTypeEntity?>
{
    public async Task<DocumentTypeEntity?> Handle(UpdateDocumentTypeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<DocumentTypeEntity>().GetByIdAsync(request.DocumentTypeId, ct);
        if (entity is null) return null;

        entity.TypeName = request.TypeName;
        entity.Abbreviation = request.Abbreviation;

        uow.Repository<DocumentTypeEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
