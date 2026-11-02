using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Empleado
{
    public int EmpleadoId { get; set; }

    public string NumeroDocumento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public string? Email { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Checkin> Checkin { get; set; } = new List<Checkin>();

    public virtual ICollection<Tripulacionvuelo> Tripulacionvuelo { get; set; } = new List<Tripulacionvuelo>();
}
