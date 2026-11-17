using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Refund
{
    public int RefundId { get; set; }

    public int PaymentId { get; set; }

    public int? TicketId { get; set; }

    public int? ProcessedByEmployeeId { get; set; }

    public decimal Amount { get; set; }

    public string? Reason { get; set; }

    public DateTime RefundDateUtc { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual Employee? ProcessedByEmployee { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
