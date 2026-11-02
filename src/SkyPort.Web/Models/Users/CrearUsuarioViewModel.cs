using System.ComponentModel.DataAnnotations;
using SkyPort.Application.Common.Constants;

namespace SkyPort.Web.Models.Users;

public sealed class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "Ingresa el correo electrónico.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(256)]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el nombre completo.")]
    [StringLength(120, MinimumLength = 2)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la contraseña.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un rol.")]
    [RegularExpression($"^({Roles.Administrador}|{Roles.OperadorVuelos}|{Roles.AgenteCheckIn})$", ErrorMessage = "Selecciona un rol válido.")]
    [Display(Name = "Rol")]
    public string Role { get; set; } = Roles.OperadorVuelos;
}
