using Microsoft.EntityFrameworkCore;
using SkyPort.Domain.Entities;
using SkyPort.Infrastructure.Repositories;
using SkyPort.Persistence.Context;

namespace SkyPort.Tests.Persistence;

public sealed class UnitOfWorkPersistenceTests : IDisposable
{
    private readonly RecordingSkyPortContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkPersistenceTests()
    {
        // Solo configura el modelo Pomelo; el contexto de grabación no abre conexión.
        var options = new DbContextOptionsBuilder<SkyPortContext>()
            .UseMySql(
                "Server=127.0.0.1;Port=3306;Database=skyport_persistence_tests;User=unused;Password=unused;",
                new MySqlServerVersion(new Version(8, 0, 36)))
            .Options;

        _context = new RecordingSkyPortContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_PreparesChange_WithoutCallingSaveChanges()
    {
        var pasajero = CreatePasajero();

        await _unitOfWork.Pasajeros.AddAsync(pasajero);

        Assert.Equal(0, _context.SaveChangesAsyncCalls);
        var entry = Assert.Single(_context.ChangeTracker.Entries<Pasajero>());
        Assert.Equal(EntityState.Added, entry.State);
    }

    [Fact]
    public async Task CompleteAsync_PersistsPreparedChanges_ThroughSharedContext()
    {
        var pasajero = CreatePasajero();
        await _unitOfWork.Pasajeros.AddAsync(pasajero);

        var changes = await _unitOfWork.CompleteAsync();

        Assert.Equal(1, changes);
        Assert.Equal(1, _context.SaveChangesAsyncCalls);
        Assert.Equal(EntityState.Unchanged, Assert.Single(_context.ChangeTracker.Entries<Pasajero>()).State);
    }

    [Fact]
    public async Task CompleteAsync_WithNoPendingChanges_ReturnsZero()
    {
        var changes = await _unitOfWork.CompleteAsync();

        Assert.Equal(0, changes);
        Assert.Equal(1, _context.SaveChangesAsyncCalls);
    }

    [Fact]
    public async Task GetByKeysAsync_FindsTripulacionvuelo_WhenKeysMatchModelOrder()
    {
        var tripulacion = new Tripulacionvuelo
        {
            VueloId = 10,
            EmpleadoId = 20,
            RolAbordo = "Piloto"
        };
        await _unitOfWork.TripulacionesVuelo.AddAsync(tripulacion);

        var primaryKey = _context.Model.FindEntityType(typeof(Tripulacionvuelo))?.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal(["VueloId", "EmpleadoId"], primaryKey.Properties.Select(property => property.Name));

        // Componentes en el orden del modelo; FindAsync resuelve en el tracker sin MySQL.
        object[] keyValues =
        [
            _context.Entry(tripulacion).Property(nameof(Tripulacionvuelo.VueloId)).CurrentValue!,
            _context.Entry(tripulacion).Property(nameof(Tripulacionvuelo.EmpleadoId)).CurrentValue!
        ];
        Assert.Equal(new object[] { 10, 20 }, keyValues);

        var found = await _unitOfWork.TripulacionesVuelo.GetByKeysAsync(keyValues);
        Assert.Same(tripulacion, found);
    }

    [Fact]
    public async Task GetByIdAsync_StillResolvesSimplePrimaryKey()
    {
        var pasajero = CreatePasajero();
        pasajero.PasajeroId = 42;
        await _unitOfWork.Pasajeros.AddAsync(pasajero);

        var found = await _unitOfWork.Pasajeros.GetByIdAsync(42);

        Assert.Same(pasajero, found);
    }

    [Fact]
    public async Task CompleteAsync_PropagatesCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _unitOfWork.CompleteAsync(cts.Token));

        Assert.Equal(0, _context.SaveChangesAsyncCalls);
    }

    [Fact]
    public async Task GetByKeysAsync_PropagatesCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _unitOfWork.TripulacionesVuelo.GetByKeysAsync([10, 20], cts.Token));
    }

    private static Pasajero CreatePasajero() => new()
    {
        TipoDocumento = "DNI",
        NumeroDocumento = "12345678",
        Nombres = "Ana",
        Apellidos = "Pérez",
        FechaNacimiento = new DateOnly(1990, 1, 15),
        Nacionalidad = "PE"
    };
}
