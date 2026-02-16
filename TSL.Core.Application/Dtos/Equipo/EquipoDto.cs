
namespace TSL.Core.Application.Dtos.Equipo
{
    public class EquipoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? Escudo { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedades calculadas
        public int CantidadPartidos { get; set; }
    }
}
