using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Tripulacionvuelo
{
    public int VueloId { get; set; }

    public int EmpleadoId { get; set; }

    public string RolAbordo { get; set; } = null!;

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual Vuelo Vuelo { get; set; } = null!;
}
