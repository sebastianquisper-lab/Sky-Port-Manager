using MediatR;
using SkyPort.Application.Ports;
using SkyPort.Application.Dtos.Reservas;

namespace SkyPort.Application.UseCases.Boletos.Queries;

public sealed record GetReservasParaBoletoQuery : IRequest<IReadOnlyList<ReservaBoletoOptionDto>>;
public sealed class GetReservasParaBoletoQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservasParaBoletoQuery, IReadOnlyList<ReservaBoletoOptionDto>>
{
    public Task<IReadOnlyList<ReservaBoletoOptionDto>> Handle(GetReservasParaBoletoQuery request, CancellationToken ct) =>
        queries.GetReservasParaBoletoAsync(ct);
}
