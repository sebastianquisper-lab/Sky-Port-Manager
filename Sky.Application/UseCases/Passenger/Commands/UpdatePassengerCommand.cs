using MediatR;
using Sky.Domain.Ports;
using PassengerEntity = Sky.Domain.Entities.Passenger;

namespace Sky.Application.UseCases.Passenger.Commands;

public sealed record UpdatePassengerCommand(
    int PassengerId,
    int DocumentTypeId,
    string DocumentNumber,
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    int? NationalityCountryId,
    string? Email,
    string? Phone) : IRequest<PassengerEntity?>;

internal sealed class UpdatePassengerHandler(IUnitOfWork uow)
    : IRequestHandler<UpdatePassengerCommand, PassengerEntity?>
{
    public async Task<PassengerEntity?> Handle(UpdatePassengerCommand request, CancellationToken ct)
    {
        var entity = await uow.Repository<PassengerEntity>().GetByIdAsync(request.PassengerId, ct);
        if (entity is null) return null;

        entity.DocumentTypeId = request.DocumentTypeId;
        entity.DocumentNumber = request.DocumentNumber;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.BirthDate = request.BirthDate;
        entity.NationalityCountryId = request.NationalityCountryId;
        entity.Email = request.Email;
        entity.Phone = request.Phone;

        uow.Repository<PassengerEntity>().Update(entity);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
