using SkyPort.Application.Dtos.Embarques;

namespace SkyPort.Application.Ports;

public interface IEmbarqueQueries
{
    Task<IReadOnlyList<EmbarqueListItemDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<EmbarqueDetailDto?> GetByIdAsync(int embarqueId, CancellationToken cancellationToken);
}
