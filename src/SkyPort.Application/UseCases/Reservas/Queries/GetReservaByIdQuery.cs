using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.Dtos.Reservas;

namespace SkyPort.Application.UseCases.Reservas.Queries;

public sealed record GetReservaByIdQuery(int ReservaId) : IRequest<Result<ReservaDto>>;

public sealed class GetReservaByIdQueryHandler(IReservaQueries queries)
    : IRequestHandler<GetReservaByIdQuery, Result<ReservaDto>>
{
    public async Task<Result<ReservaDto>> Handle(GetReservaByIdQuery request, CancellationToken cancellationToken)
    {
        var reserva = await queries.GetByIdAsync(request.ReservaId, cancellationToken);
        return reserva is null
            ? Result<ReservaDto>.Failure(Error.NotFound("No se encontró la reserva."))
            : Result<ReservaDto>.Success(reserva);
    }
}
