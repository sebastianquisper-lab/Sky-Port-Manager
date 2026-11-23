using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

/// <summary>
/// Pasarela SIMULADA para el proyecto académico (no mueve dinero).
/// Rechaza el cobro si no hay token o si el token empieza con "tok_fail"; así se puede probar el flujo FAILED.
/// Para una pasarela real (Stripe, Culqi, etc.) basta con otra clase que implemente IPaymentGateway.
/// </summary>
public sealed class SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger) : IPaymentGateway
{
    public Task<string?> ChargeAsync(
        decimal amount,
        string currencyCode,
        string description,
        string? customerEmail,
        string paymentMethod,
        string? paymentToken,
        CancellationToken ct = default)
    {
        if (amount <= 0
            || string.IsNullOrWhiteSpace(paymentToken)
            || paymentToken.StartsWith("tok_fail", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Cobro simulado RECHAZADO: {Description} ({Amount} {Currency})", description, amount, currencyCode);
            return Task.FromResult<string?>(null);
        }

        var reference = $"SIM-{Guid.NewGuid():N}"[..20];
        logger.LogInformation("Cobro simulado APROBADO {Reference}: {Description} ({Amount} {Currency})", reference, description, amount, currencyCode);
        return Task.FromResult<string?>(reference);
    }

    public Task<bool> RefundAsync(
        string transactionReference,
        decimal amount,
        string currencyCode,
        CancellationToken ct = default)
    {
        var accepted = !string.IsNullOrWhiteSpace(transactionReference) && amount > 0;
        logger.LogInformation("Reembolso simulado {Result} para {Reference} ({Amount} {Currency})",
            accepted ? "ACEPTADO" : "RECHAZADO", transactionReference, amount, currencyCode);
        return Task.FromResult(accepted);
    }
}
