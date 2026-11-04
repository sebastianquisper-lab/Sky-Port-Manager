using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Equipajes;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Equipajes.Queries;

public sealed record GetEquipajeByIdQuery(int EquipajeId) : IRequest<Result<EquipajeDto>>;

public sealed class GetEquipajeByIdQueryHandler(IEquipajeQueries queries)
    : IRequestHandler<GetEquipajeByIdQuery, Result<EquipajeDto>>
{
    public async Task<Result<EquipajeDto>> Handle(GetEquipajeByIdQuery request, CancellationToken cancellationToken)
    {
        var baggage = await queries.GetByIdAsync(request.EquipajeId, cancellationToken);
        return baggage is null
            ? Result<EquipajeDto>.Failure(Error.NotFound("Equipaje no encontrado."))
            : Result<EquipajeDto>.Success(baggage);
    }
}
