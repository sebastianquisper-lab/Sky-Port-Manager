using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.CheckIn.Queries;

public sealed record GetCheckInsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<CheckInDto>>;
public sealed class GetCheckInsQueryHandler(ICheckInQueries queries) : IRequestHandler<GetCheckInsQuery, PagedResult<CheckInDto>>
{
    public Task<PagedResult<CheckInDto>> Handle(GetCheckInsQuery request, CancellationToken ct) =>
        queries.GetPageAsync(Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), ct);
}
