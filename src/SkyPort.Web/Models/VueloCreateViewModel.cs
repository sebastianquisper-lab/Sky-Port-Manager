using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Models;

public sealed class VueloCreateViewModel : IValidatableObject
{
    [Required, StringLength(10, MinimumLength = 1)]
    [Display(Name = "Número de vuelo")]
    public string NumeroVuelo { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Ruta")]
    public int RutaId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Aeronave")]
    public int AeronaveId { get; set; }

    [Display(Name = "Puerta (opcional)")]
    public int? PuertaId { get; set; }

    [Required]
    [Display(Name = "Fecha y hora de salida (hora local)")]
    public DateTime? FechaSalida { get; set; }

    [Required]
    [Display(Name = "Fecha y hora de llegada (hora local)")]
    public DateTime? FechaLlegada { get; set; }

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Rutas { get; set; } = [];

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Aeronaves { get; set; } = [];

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Puertas { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaSalida.HasValue && FechaLlegada.HasValue && FechaLlegada.Value <= FechaSalida.Value)
        {
            yield return new ValidationResult(
                "La llegada debe ser posterior a la salida.",
                [nameof(FechaLlegada)]);
        }
    }
}
