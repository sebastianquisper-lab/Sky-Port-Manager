using MediatR;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Embarques.Queries;

public sealed record GetEmbarquesQuery : IRequest<IReadOnlyList<EmbarqueListItemDto>>;

public sealed class GetEmbarquesQueryHandler(IEmbarqueQueries queries)
    : IRequestHandler<GetEmbarquesQuery, IReadOnlyList<EmbarqueListItemDto>>
{
    public Task<IReadOnlyList<EmbarqueListItemDto>> Handle(GetEmbarquesQuery request, CancellationToken cancellationToken) =>
        queries.GetAllAsync(cancellationToken);
}
