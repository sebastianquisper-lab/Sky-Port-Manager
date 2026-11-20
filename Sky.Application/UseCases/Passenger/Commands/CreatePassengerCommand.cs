using MediatR;
using Sky.Domain.Ports;
using PassengerEntity = Sky.Domain.Entities.Passenger;

namespace Sky.Application.UseCases.Passenger.Commands;

public sealed record CreatePassengerCommand(
    int DocumentTypeId,
    string DocumentNumber,
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    int? NationalityCountryId,
    string? Email,
    string? Phone) : IRequest<PassengerEntity>;

internal sealed class CreatePassengerHandler(IUnitOfWork uow)
    : IRequestHandler<CreatePassengerCommand, PassengerEntity>
{
    public async Task<PassengerEntity> Handle(CreatePassengerCommand request, CancellationToken ct)
    {
        var entity = new PassengerEntity
        {
            DocumentTypeId = request.DocumentTypeId,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            NationalityCountryId = request.NationalityCountryId,
            Email = request.Email,
            Phone = request.Phone
        };

        await uow.Repository<PassengerEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
