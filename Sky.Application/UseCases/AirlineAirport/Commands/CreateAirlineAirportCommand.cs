using MediatR;
using Sky.Domain.Ports;
using AirlineAirportEntity = Sky.Domain.Entities.AirlineAirport;

namespace Sky.Application.UseCases.AirlineAirport.Commands;

public sealed record CreateAirlineAirportCommand(
    int AirlineId,
    int AirportId,
    DateOnly SinceDate,
    bool IsHub) : IRequest<AirlineAirportEntity>;

internal sealed class CreateAirlineAirportHandler(IUnitOfWork uow)
    : IRequestHandler<CreateAirlineAirportCommand, AirlineAirportEntity>
{
    public async Task<AirlineAirportEntity> Handle(CreateAirlineAirportCommand request, CancellationToken ct)
    {
        var entity = new AirlineAirportEntity
        {
            AirlineId = request.AirlineId,
            AirportId = request.AirportId,
            SinceDate = request.SinceDate,
            IsHub = request.IsHub
        };

        await uow.Repository<AirlineAirportEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity;
    }
}
