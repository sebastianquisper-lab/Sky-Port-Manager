using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Fare
{
    public int FareId { get; set; }

    public int AirlineId { get; set; }

    public int RouteId { get; set; }

    public int CabinClassId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public bool IsActive { get; set; }

    public virtual Airline Airline { get; set; } = null!;

    public virtual CabinClass CabinClass { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual FlightRoute Route { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
