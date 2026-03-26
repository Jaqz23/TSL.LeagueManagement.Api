using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Account
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Número de teléfono inválido")]
        public string? PhoneNumber { get; set; }
    }
}
