using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using SkyPort.Application.Ports;
using SkyPort.Domain.Ports;
using SkyPort.Infrastructure.Repositories;
using SkyPort.Infrastructure.Services;
using SkyPort.Persistence.Context;
using SkyPort.Persistence.Identity;

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
        services.AddSkyPortIdentity();
        services.AddDbContext<SkyPortIdentityDbContext>(options => options.UseMySql(connectionString, serverVersion));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        RegisterApplicationPortImplementations(services);

        return services;
    }

    public static IServiceCollection AddSkyPortIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<SkyPortIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

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
