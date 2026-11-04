using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class UsuarioPerfilViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(256)]
    [Display(Name = "Nombre de usuario")]
    public string UserName { get; set; } = string.Empty;
}
