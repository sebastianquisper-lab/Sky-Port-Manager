using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class DocumentType
{
    public int DocumentTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}
