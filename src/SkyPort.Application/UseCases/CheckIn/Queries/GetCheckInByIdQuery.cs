using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.CheckIn.Queries;

public sealed record GetCheckInByIdQuery(int CheckInId) : IRequest<Result<CheckInDto>>;
public sealed class GetCheckInByIdQueryHandler(ICheckInQueries queries) : IRequestHandler<GetCheckInByIdQuery, Result<CheckInDto>>
{
    public async Task<Result<CheckInDto>> Handle(GetCheckInByIdQuery request, CancellationToken ct)
    {
        var checkIn = await queries.GetByIdAsync(request.CheckInId, ct);
        return checkIn is null ? Result<CheckInDto>.Failure(Error.NotFound("No se encontró el check-in.")) : Result<CheckInDto>.Success(checkIn);
    }
}
