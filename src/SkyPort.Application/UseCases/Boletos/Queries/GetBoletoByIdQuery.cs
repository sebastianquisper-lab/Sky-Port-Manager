using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Boletos;
using SkyPort.Application.Ports;

namespace SkyPort.Application.UseCases.Boletos.Queries;

public sealed record GetBoletoByIdQuery(int BoletoId) : IRequest<Result<BoletoDto>>;
public sealed class GetBoletoByIdQueryHandler(IBoletoQueries queries) : IRequestHandler<GetBoletoByIdQuery, Result<BoletoDto>>
{
    public async Task<Result<BoletoDto>> Handle(GetBoletoByIdQuery request, CancellationToken ct)
    {
        var boleto = await queries.GetByIdAsync(request.BoletoId, ct);
        return boleto is null ? Result<BoletoDto>.Failure(Error.NotFound("No se encontró el boleto.")) : Result<BoletoDto>.Success(boleto);
    }
}
