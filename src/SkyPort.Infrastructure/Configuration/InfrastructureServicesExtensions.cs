using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using SkyPort.Application.Ports;
using SkyPort.Domain.Ports;
using SkyPort.Infrastructure.Repositories;
using SkyPort.Infrastructure.Services;
using SkyPort.Persistence.Context;

namespace SkyPort.Infrastructure.Configuration;

public static class InfrastructureServicesExtensions
{
    private const string ApplicationPortsNamespace = "SkyPort.Application.Ports";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ConnectionStrings:DefaultConnection' is not configured.");
        }

        var serverVersion = DetectServerVersion(connectionString);
        services.AddDbContext<SkyPortContext>(options => options.UseMySql(connectionString, serverVersion));
        services.AddDbContext<SkyPortIdentityDbContext>(options => options.UseMySql(connectionString, serverVersion));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        RegisterApplicationPortImplementations(services);

        return services;
    }

    private static ServerVersion DetectServerVersion(string connectionString)
    {
        try
        {
            return ServerVersion.AutoDetect(connectionString);
        }
        catch (MySqlException)
        {
            return new MySqlServerVersion(new Version(8, 0, 36));
        }
    }

    private static void RegisterApplicationPortImplementations(IServiceCollection services)
    {
        var applicationPortTypes = typeof(IPasswordHasher).Assembly
            .GetTypes()
            .Where(type => type.IsInterface && type.Namespace == ApplicationPortsNamespace)
            .ToHashSet();

        var implementationNamespaces = new HashSet<string>(StringComparer.Ordinal)
        {
            "SkyPort.Infrastructure.Queries",
            "SkyPort.Infrastructure.Services"
        };

        var implementations = typeof(Pbkdf2PasswordHasher).Assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Namespace is not null &&
                implementationNamespaces.Contains(type.Namespace));

        foreach (var implementation in implementations)
        {
            foreach (var port in implementation.GetInterfaces().Where(applicationPortTypes.Contains))
            {
                services.AddScoped(port, implementation);
            }
        }
    }
}
