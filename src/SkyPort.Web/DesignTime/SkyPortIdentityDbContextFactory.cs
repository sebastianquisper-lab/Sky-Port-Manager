using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySqlConnector;
using SkyPort.Persistence.Context;

namespace SkyPort.Web.DesignTime;

public sealed class SkyPortIdentityDbContextFactory : IDesignTimeDbContextFactory<SkyPortIdentityDbContext>
{
    public SkyPortIdentityDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SkyPortIdentityDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configura ConnectionStrings:DefaultConnection en user-secrets de SkyPort.Web " +
                "o en la variable ConnectionStrings__DefaultConnection antes de generar el script.");
        }

        using var connection = new MySqlConnection(connectionString);
        if (!string.Equals(connection.Database, "skyport_db", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("La conexion debe apuntar a skyport_db, creada previamente por J02.");
        }

        // Abrir la base indicada confirma que existe; no se crea ninguna base desde EF.
        connection.Open();
        var serverVersion = ServerVersion.AutoDetect(connection);
        if (serverVersion is not MySqlServerVersion || serverVersion.Version < new Version(8, 0, 16))
        {
            throw new InvalidOperationException("Se requiere un servidor MySQL 8.0.16 o superior.");
        }

        var options = new DbContextOptionsBuilder<SkyPortIdentityDbContext>()
            .UseMySql(connectionString, serverVersion)
            .Options;

        return new SkyPortIdentityDbContext(options);
    }
}
