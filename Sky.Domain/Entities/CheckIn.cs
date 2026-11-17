using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class CheckIn
{
    public int CheckInId { get; set; }

    public int TicketId { get; set; }

    public int? CounterId { get; set; }

    public int? EmployeeId { get; set; }

    public string Channel { get; set; } = null!;

    public DateTime CheckInTimeUtc { get; set; }

    public virtual CheckInCounter? Counter { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
