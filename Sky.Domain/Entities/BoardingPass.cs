using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BoardingPass
{
    public int BoardingPassId { get; set; }

    public int TicketId { get; set; }

    public string? BoardingGroup { get; set; }

    public short? SequenceNumber { get; set; }

    public string Barcode { get; set; } = null!;

    public DateTime IssuedAtUtc { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
