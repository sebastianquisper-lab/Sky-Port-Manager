using MediatR;
using SkyPort.Application.Dtos.Embarques;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Embarques.Queries;

public sealed record GetEmbarqueCreateOptionsQuery : IRequest<EmbarqueCreateOptionsDto>;

public sealed class GetEmbarqueCreateOptionsQueryHandler(IEmbarqueQueries queries)
    : IRequestHandler<GetEmbarqueCreateOptionsQuery, EmbarqueCreateOptionsDto>
{
    public Task<EmbarqueCreateOptionsDto> Handle(GetEmbarqueCreateOptionsQuery request, CancellationToken cancellationToken) =>
        queries.GetCreateOptionsAsync(cancellationToken);
}
