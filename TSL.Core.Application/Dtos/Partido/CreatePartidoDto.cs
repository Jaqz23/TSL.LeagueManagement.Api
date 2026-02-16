using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Partido
{
    public class CreatePartidoDto
    {
        [Required(ErrorMessage = "La fecha del partido es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La jornada es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La jornada debe ser mayor a 0")]
        public int Jornada { get; set; }

        [Required(ErrorMessage = "El ID de la temporada es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la temporada debe ser mayor a 0")]
        public int TemporadaId { get; set; }

        [Required(ErrorMessage = "El ID del equipo local es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del equipo local debe ser mayor a 0")]
        public int EquipoLocalId { get; set; }

        [Required(ErrorMessage = "El ID del equipo visitante es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del equipo visitante debe ser mayor a 0")]
        public int EquipoVisitanteId { get; set; }
    }
}
