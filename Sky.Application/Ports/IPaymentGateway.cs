namespace Sky.Application.Ports;

public interface IPaymentGateway
{
    /// <summary>
    /// Cobra <paramref name="amount"/>. Devuelve la referencia de la transacción si la pasarela aprobó el cobro,
    /// o null si lo rechazó. <paramref name="paymentToken"/> es el token de tarjeta/billetera (nunca el número).
    /// </summary>
    Task<string?> ChargeAsync(
        decimal amount,
        string currencyCode,
        string description,
        string? customerEmail,
        string paymentMethod,
        string? paymentToken,
        CancellationToken ct = default);

    /// <summary>Reembolsa parte o la totalidad de una transacción. Devuelve true si la pasarela lo aceptó.</summary>
    Task<bool> RefundAsync(
        string transactionReference,
        decimal amount,
        string currencyCode,
        CancellationToken ct = default);
}
