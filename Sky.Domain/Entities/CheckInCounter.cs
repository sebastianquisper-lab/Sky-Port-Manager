using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class CheckInCounter
{
    public int CounterId { get; set; }

    public int TerminalId { get; set; }

    public string CounterCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public virtual Terminal Terminal { get; set; } = null!;
}
