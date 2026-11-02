using SkyPort.Domain.Entities;

namespace SkyPort.Domain.Ports;

public interface IUnitOfWork
{
    IGenericRepository<Aerolinea> Aerolineas { get; }

    IGenericRepository<Aeronave> Aeronaves { get; }

    IGenericRepository<Boleto> Boletos { get; }

    IGenericRepository<Checkin> CheckIns { get; }

    IGenericRepository<Embarque> Embarques { get; }

    IGenericRepository<Empleado> Empleados { get; }

    IGenericRepository<Equipaje> Equipajes { get; }

    IGenericRepository<Pasajero> Pasajeros { get; }

    IGenericRepository<Puerta> Puertas { get; }

    IGenericRepository<Reserva> Reservas { get; }

    IGenericRepository<Ruta> Rutas { get; }

    IGenericRepository<Terminal> Terminales { get; }

    IGenericRepository<Tripulacionvuelo> TripulacionesVuelo { get; }

    IGenericRepository<Vuelo> Vuelos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
