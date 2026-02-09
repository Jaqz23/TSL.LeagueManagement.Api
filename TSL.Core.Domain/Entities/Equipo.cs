using TSL.Core.Domain.Common;

namespace TSL.Core.Domain.Entities
{
    public class Equipo : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;

        public string? Ciudad { get; set; }

        // URL o ruta del escudo
        public string? Escudo { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation property


        // Partidos donde este equipo juega como local
        public virtual ICollection<Partido> PartidosLocal { get; set; } = new HashSet<Partido>();

        // Partidos donde este equipo juega como visitante
        public virtual ICollection<Partido> PartidosVisitante { get; set; } = new HashSet<Partido>();


        // Posiciones del equipo en diferentes tablas
        public virtual ICollection<PosicionEquipo> Posiciones { get; set; } = new HashSet<PosicionEquipo>();

        #region Metodos de negocio

        // Valida si el equipo puede ser eliminado
        // Regla: No se puede eliminar si tiene partidos
        public bool PuedeSerEliminado() 
        {
            return !PartidosLocal.Any() && !PartidosVisitante.Any();
        }

        // Obtiene todos los partidos del equipo (local + visitante)
        public IEnumerable<Partido> ObtenerTodosLosPartidos() 
        {
            return PartidosLocal.Union(PartidosVisitante);
        }

        #endregion
    }
}
