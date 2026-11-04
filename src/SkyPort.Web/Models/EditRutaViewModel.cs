using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class EditRutaViewModel
{
    public int RutaId { get; set; }

    public string CodigoOrigen { get; set; } = string.Empty;

    public string CiudadOrigen { get; set; } = string.Empty;

    public string CodigoDestino { get; set; } = string.Empty;

    public string CiudadDestino { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Duración en minutos")]
    public int DuracionMinutos { get; set; }
}
