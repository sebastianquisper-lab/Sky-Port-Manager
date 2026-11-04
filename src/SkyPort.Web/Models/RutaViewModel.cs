using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class RutaViewModel : IValidatableObject
{
    [Required, StringLength(3, MinimumLength = 1)]
    [Display(Name = "Código de origen")]
    public string CodigoOrigen { get; set; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    [Display(Name = "Ciudad de origen")]
    public string CiudadOrigen { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 1)]
    [Display(Name = "Código de destino")]
    public string CodigoDestino { get; set; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    [Display(Name = "Ciudad de destino")]
    public string CiudadDestino { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Duración en minutos")]
    public int DuracionMinutos { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(CodigoOrigen) &&
            string.Equals(CodigoOrigen.Trim(), CodigoDestino?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "El origen y el destino deben ser distintos.",
                [nameof(CodigoDestino)]);
        }
    }
}
