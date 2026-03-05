using System.ComponentModel.DataAnnotations;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Dtos.Partido
{
    public class UpdatePartidoDto
    {
        [Required(ErrorMessage = "La fecha del partido es obligatoria")]
        public DateTime? Fecha { get; set; }

        [Required(ErrorMessage = "La jornada es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La jornada debe ser mayor a 0")]
        public int? Jornada { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Los goles no pueden ser negativos")]
        public int? GolesLocal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Los goles no pueden ser negativos")]
        public int? GolesVisitante { get; set; }

        public EstadoPartido Estado { get; set; }
    }
}
