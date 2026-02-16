using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Partido
{
    public class RegistrarResultadoDto
    {
        [Required(ErrorMessage = "Los goles del equipo local son obligatorios")]
        [Range(0, int.MaxValue, ErrorMessage = "Los goles no pueden ser negativos")]
        public int GolesLocal { get; set; }

        [Required(ErrorMessage = "Los goles del equipo visitante son obligatorios")]
        [Range(0, int.MaxValue, ErrorMessage = "Los goles no pueden ser negativos")]
        public int GolesVisitante { get; set; }
    }
}
