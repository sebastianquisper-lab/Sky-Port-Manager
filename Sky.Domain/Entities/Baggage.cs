using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Baggage
{
    public int BaggageId { get; set; }

    public int TicketId { get; set; }

    public int BaggageTypeId { get; set; }

    public int BaggageStatusId { get; set; }

    public string TagNumber { get; set; } = null!;

    public decimal WeightKg { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public virtual BaggageStatus BaggageStatus { get; set; } = null!;

    public virtual ICollection<BaggageTracking> BaggageTrackings { get; set; } = new List<BaggageTracking>();

    public virtual BaggageType BaggageType { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
