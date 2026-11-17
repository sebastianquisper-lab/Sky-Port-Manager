using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class TicketStatus
{
    public int TicketStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
