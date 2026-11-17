using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int PaymentMethodId { get; set; }

    public int PaymentStatusId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? TransactionReference { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? PaidAtUtc { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
