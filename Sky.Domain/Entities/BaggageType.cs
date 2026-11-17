using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BaggageType
{
    public int BaggageTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal? MaxWeightKg { get; set; }

    public virtual ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
}
