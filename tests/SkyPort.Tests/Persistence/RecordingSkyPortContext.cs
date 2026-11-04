using Microsoft.EntityFrameworkCore;
using SkyPort.Persistence.Context;

namespace SkyPort.Tests.Persistence;

/// <summary>
/// Contexto de prueba que evita tocar MySQL: registra llamadas a SaveChangesAsync
/// y acepta el change tracker en memoria.
/// </summary>
internal sealed class RecordingSkyPortContext : SkyPortContext
{
    public RecordingSkyPortContext(DbContextOptions<SkyPortContext> options)
        : base(options)
    {
    }

    public int SaveChangesAsyncCalls { get; private set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SaveChangesAsyncCalls++;

        var pending = ChangeTracker.Entries()
            .Count(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        ChangeTracker.AcceptAllChanges();
        return Task.FromResult(pending);
    }
}
