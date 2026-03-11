using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Equipo
{
    public class CreateEquipoDto
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad del equipo es obligatoria")]
        [StringLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres")]
        public string? Ciudad { get; set; }

        [StringLength(500, ErrorMessage = "La URL del escudo es demasiado larga")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string? Escudo { get; set; }
    }
}
