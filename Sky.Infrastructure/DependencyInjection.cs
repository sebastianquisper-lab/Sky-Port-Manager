using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sky.Application.Ports;
using Sky.Domain.Ports;
using Sky.Infrastructure.Adapters;
using Sky.Infrastructure.Adapters.Services;
using Sky.Infrastructure.Context;

namespace Sky.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SkyportDb")
            ?? throw new InvalidOperationException(
                "Falta ConnectionStrings:SkyportDb (usa user-secrets o una variable de entorno).");

        // Único punto que depende del motor de base de datos.
        services.AddDbContext<SkyportDbContext>(options => options.UseNpgsql(connectionString));
        // MySQL (Pomelo):  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpContextAccessor();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();
        services.AddSingleton<IBoardingPassBarcodeGenerator, BoardingPassBarcodeGenerator>();
        services.AddSingleton<IPaymentGateway, SimulatedPaymentGateway>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<IDocumentGenerator, QuestPdfDocumentGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
