using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Embarques.Queries;

public sealed record GetEmbarqueByIdQuery(int EmbarqueId) : IRequest<Result<EmbarqueDetailDto>>;

public sealed class GetEmbarqueByIdQueryHandler(IEmbarqueQueries queries)
    : IRequestHandler<GetEmbarqueByIdQuery, Result<EmbarqueDetailDto>>
{
    public async Task<Result<EmbarqueDetailDto>> Handle(GetEmbarqueByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.EmbarqueId <= 0)
        {
            return Result<EmbarqueDetailDto>.Failure(Error.NotFound("No se encontró el embarque."));
        }

        var boarding = await queries.GetByIdAsync(request.EmbarqueId, cancellationToken);
        return boarding is null
            ? Result<EmbarqueDetailDto>.Failure(Error.NotFound("No se encontró el embarque."))
            : Result<EmbarqueDetailDto>.Success(boarding);
    }
}
