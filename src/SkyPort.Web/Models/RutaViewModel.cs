using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class RutaViewModel
{
    public int RutaId { get; set; }

    [Required(ErrorMessage = "Ingresa el código de origen.")]
    [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "El código debe tener 3 letras.")]
    [Display(Name = "Código de origen")]
    public string CodigoOrigen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la ciudad de origen.")]
    [StringLength(80)]
    [Display(Name = "Ciudad de origen")]
    public string CiudadOrigen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el código de destino.")]
    [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "El código debe tener 3 letras.")]
    [Display(Name = "Código de destino")]
    public string CodigoDestino { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la ciudad de destino.")]
    [StringLength(80)]
    [Display(Name = "Ciudad de destino")]
    public string CiudadDestino { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor a cero minutos.")]
    [Display(Name = "Duración (minutos)")]
    public int DuracionMinutos { get; set; }
}
