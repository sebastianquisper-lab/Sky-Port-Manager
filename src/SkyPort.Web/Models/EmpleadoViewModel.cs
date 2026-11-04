using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Models;

public sealed class EmpleadoViewModel
{
    public int EmpleadoId { get; set; }

    [Required, StringLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Apellidos { get; set; } = string.Empty;

    [Required]
    public string Cargo { get; set; } = string.Empty;

    [EmailAddress, StringLength(120)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Indica si el empleado está activo.")]
    public bool? Activo { get; set; }

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Cargos { get; set; } = [];
}
