using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Puertas;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Puertas.Queries;

public sealed record GetPuertaByIdQuery(int PuertaId) : IRequest<Result<PuertaDto>>;

public sealed class GetPuertaByIdQueryHandler(IPuertaQueries queries)
    : IRequestHandler<GetPuertaByIdQuery, Result<PuertaDto>>
{
    public async Task<Result<PuertaDto>> Handle(GetPuertaByIdQuery request, CancellationToken cancellationToken)
    {
        var gate = await queries.GetByIdAsync(request.PuertaId, cancellationToken);
        return gate is null
            ? Result<PuertaDto>.Failure(Error.NotFound("Puerta no encontrada."))
            : Result<PuertaDto>.Success(gate);
    }
}
