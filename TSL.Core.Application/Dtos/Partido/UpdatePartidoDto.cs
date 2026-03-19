using System.ComponentModel.DataAnnotations;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Dtos.Partido
{
    public class UpdatePartidoDto
    {

        [Required(ErrorMessage = "La fecha del partido es obligatoria")]
        public DateTime? Fecha { get; set; }

        [Required(ErrorMessage = "La jornada es obligatoria")]
        [Range(1, 30, ErrorMessage = "La jornada debe estar entre 1 y 30")]
        public int? Jornada { get; set; }

        [Required(ErrorMessage = "El ID de la temporada es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "ID de temporada inválido")]
        public int TemporadaId { get; set; }

        [Required(ErrorMessage = "El ID del equipo local es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "ID de equipo local inválido")]
        public int EquipoLocalId { get; set; }

        [Required(ErrorMessage = "El ID del equipo visitante es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "ID de equipo visitante inválido")]
        public int EquipoVisitanteId { get; set; }

    }
}
