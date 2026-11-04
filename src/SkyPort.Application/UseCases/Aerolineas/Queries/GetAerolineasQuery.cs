using MediatR;
using SkyPort.Application.Dtos.Aerolineas;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Aerolineas.Queries;

public sealed record GetAerolineasQuery : IRequest<IReadOnlyList<AerolineaDto>>;

public sealed class GetAerolineasQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAerolineasQuery, IReadOnlyList<AerolineaDto>>
{
    public async Task<IReadOnlyList<AerolineaDto>> Handle(
        GetAerolineasQuery request,
        CancellationToken cancellationToken)
    {
        var aerolineas = await unitOfWork.Aerolineas.GetAllAsync(cancellationToken);
        return aerolineas
            .OrderBy(aerolinea => aerolinea.Nombre)
            .Select(aerolinea => new AerolineaDto(
                aerolinea.AerolineaId,
                aerolinea.Nombre,
                aerolinea.CodigoIata,
                aerolinea.Pais,
                aerolinea.Activa ?? false))
            .ToList();
    }
}
