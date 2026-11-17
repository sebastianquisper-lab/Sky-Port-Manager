using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Boarding
{
    public int BoardingId { get; set; }

    public int TicketId { get; set; }

    public int GateId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime BoardedAtUtc { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual BoardingGate Gate { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
