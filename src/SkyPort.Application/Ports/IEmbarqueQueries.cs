using SkyPort.Application.Dtos.Embarques;

namespace SkyPort.Application.Ports;

public interface IEmbarqueQueries
{
    Task<IReadOnlyList<EmbarqueListItemDto>> GetAllAsync(CancellationToken cancellationToken);
}
