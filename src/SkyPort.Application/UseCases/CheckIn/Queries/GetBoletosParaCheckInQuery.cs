using MediatR;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.CheckIn.Queries;

public sealed record GetBoletosParaCheckInQuery : IRequest<IReadOnlyList<BoletoCheckInOptionDto>>;
public sealed class GetBoletosParaCheckInQueryHandler(IBoletoQueries queries)
    : IRequestHandler<GetBoletosParaCheckInQuery, IReadOnlyList<BoletoCheckInOptionDto>>
{
    public Task<IReadOnlyList<BoletoCheckInOptionDto>> Handle(GetBoletosParaCheckInQuery request, CancellationToken ct) => queries.GetForCheckInAsync(ct);
}
