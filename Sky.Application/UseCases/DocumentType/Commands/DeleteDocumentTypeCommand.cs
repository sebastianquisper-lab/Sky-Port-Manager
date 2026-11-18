using MediatR;
using Sky.Domain.Ports;
using DocumentTypeEntity = Sky.Domain.Entities.DocumentType;

namespace Sky.Application.UseCases.DocumentType.Commands;

public sealed record DeleteDocumentTypeCommand(
    int DocumentTypeId) : IRequest<bool>;

internal sealed class DeleteDocumentTypeHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteDocumentTypeCommand, bool>
{
    public async Task<bool> Handle(DeleteDocumentTypeCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<DocumentTypeEntity>().GetByIdAsync(request.DocumentTypeId, ct);
        if (entity is null) return false;

        uow.Repository<DocumentTypeEntity>().Delete(entity);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
