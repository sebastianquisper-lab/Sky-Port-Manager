using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SkyPortContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(SkyPortContext context)
    {
        _context = context;
    }

    public IGenericRepository<Aerolinea> Aerolineas => Repository<Aerolinea>();

    public IGenericRepository<Aeronave> Aeronaves => Repository<Aeronave>();

    public IGenericRepository<Boleto> Boletos => Repository<Boleto>();

    public IGenericRepository<Checkin> CheckIns => Repository<Checkin>();

    public IGenericRepository<Embarque> Embarques => Repository<Embarque>();

    public IGenericRepository<Empleado> Empleados => Repository<Empleado>();

    public IGenericRepository<Equipaje> Equipajes => Repository<Equipaje>();

    public IGenericRepository<Pasajero> Pasajeros => Repository<Pasajero>();

    public IGenericRepository<Puerta> Puertas => Repository<Puerta>();

    public IGenericRepository<Reserva> Reservas => Repository<Reserva>();

    public IGenericRepository<Ruta> Rutas => Repository<Ruta>();

    public IGenericRepository<Terminal> Terminales => Repository<Terminal>();

    public IGenericRepository<Tripulacionvuelo> TripulacionesVuelo => Repository<Tripulacionvuelo>();

    public IGenericRepository<Vuelo> Vuelos => Repository<Vuelo>();

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is MySqlException { Number: 1451 or 1452 })
                throw new ReferenceConflictException(exception);

            if (exception.InnerException is MySqlException
                {
                    ErrorCode: MySqlErrorCode.DuplicateKeyEntry
                } mysqlException)
            {
                throw new DuplicateKeyException(mysqlException.Message, exception);
            }

            throw;
        }
    }

    private IGenericRepository<T> Repository<T>() where T : class
    {
        if (!_repositories.TryGetValue(typeof(T), out var repository))
        {
            repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
        }

        return (IGenericRepository<T>)repository;
    }
}
