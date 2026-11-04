using SkyPort.Application.UseCases.Reservas.Dtos;
using SkyPort.Application.Common;

namespace SkyPort.Application.Ports;

public interface IReservaQueries
{
    Task<PagedResult<ReservaDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<ReservaDto?> GetByIdAsync(int reservaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaBoletoOptionDto>> GetReservasParaBoletoAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VueloAbiertoDto>> GetVuelosAbiertosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaDto>> GetByVueloAsync(int vueloId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReservaDto>> GetByPasajeroAsync(int pasajeroId, CancellationToken cancellationToken = default);

    Task<AsientosVueloDto?> GetAsientosAsync(int vueloId, CancellationToken cancellationToken = default);
}
