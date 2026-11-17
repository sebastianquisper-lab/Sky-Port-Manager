using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftModel
{
    public int ModelId { get; set; }

    public int ManufacturerId { get; set; }

    public string ModelName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();

    public virtual AircraftManufacturer Manufacturer { get; set; } = null!;
}
