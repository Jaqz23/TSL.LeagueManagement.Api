using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Users
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El email o nombre de usuario es obligatorio")]
        [Display(Name = "Email o Usuario")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        public string? Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
