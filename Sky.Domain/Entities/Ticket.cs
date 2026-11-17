using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int BookingId { get; set; }

    public int FlightId { get; set; }

    public int PassengerId { get; set; }

    public int TicketStatusId { get; set; }

    public int CabinClassId { get; set; }

    public int? FareId { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string? SeatNumber { get; set; }

    public DateTime IssueDateUtc { get; set; }

    public decimal Price { get; set; }

    public decimal TaxAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public virtual ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();

    public virtual Boarding? Boarding { get; set; }

    public virtual BoardingPass? BoardingPass { get; set; }

    public virtual BookingFlightPassenger BookingFlightPassenger { get; set; } = null!;

    public virtual CabinClass CabinClass { get; set; } = null!;

    public virtual CheckIn? CheckIn { get; set; }

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual Fare? Fare { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual TicketStatus TicketStatus { get; set; } = null!;
}
