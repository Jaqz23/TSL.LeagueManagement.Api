
namespace TSL.Core.Application.Dtos.Temporada
{
    public class TemporadaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Estado { get; set; }

        // Relacion con Liga
        public int LigaId { get; set; }
        public string NombreLiga { get; set; } = string.Empty;

        // Propiedades calculadas
        public int CantidadPartidos { get; set; }
        public string EstadoTexto => Estado ? "Activa" : "Finalizada";
        public int DuracionDias => (FechaFin - FechaInicio).Days;

    }
}
