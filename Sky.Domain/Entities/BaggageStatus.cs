using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BaggageStatus
{
    public int BaggageStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<BaggageTracking> BaggageTrackings { get; set; } = new List<BaggageTracking>();

    public virtual ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
}
