using MediatR;
using SkyPort.Application.Dtos.CheckIn;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.CheckIn.Queries;

public sealed record GetCheckInEmployeesQuery : IRequest<IReadOnlyList<CheckInEmployeeOptionDto>>;
public sealed class GetCheckInEmployeesQueryHandler(ICheckInQueries queries)
    : IRequestHandler<GetCheckInEmployeesQuery, IReadOnlyList<CheckInEmployeeOptionDto>>
{
    public Task<IReadOnlyList<CheckInEmployeeOptionDto>> Handle(GetCheckInEmployeesQuery request, CancellationToken ct) => queries.GetEmployeesAsync(ct);
}
