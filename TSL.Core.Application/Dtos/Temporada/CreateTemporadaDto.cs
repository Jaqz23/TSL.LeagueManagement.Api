using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Temporada
{
    public class CreateTemporadaDto
    {
        [Required(ErrorMessage = "El nombre de la temporada es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El ID de la liga es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la liga debe ser mayor a 0")]
        public int LigaId { get; set; }
    }
}
