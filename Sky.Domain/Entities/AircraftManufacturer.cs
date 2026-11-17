using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftManufacturer
{
    public int ManufacturerId { get; set; }

    public string ManufacturerName { get; set; } = null!;

    public virtual ICollection<AircraftModel> AircraftModels { get; set; } = new List<AircraftModel>();
}
