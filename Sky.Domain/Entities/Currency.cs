using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Currency
{
    public string CurrencyCode { get; set; } = null!;

    public string CurrencyName { get; set; } = null!;

    public string? Symbol { get; set; }

    public virtual ICollection<Fare> Fares { get; set; } = new List<Fare>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
