using TSL.Core.Domain.Common;

namespace TSL.Core.Domain.Entities
{
    public class TablaPosicion : BaseEntity
    {
        public int TemporadaId { get; set; }

        // Navigation property

        // Temporada a la que pertenece
        public virtual Temporada Temporada { get; set; } = null!;

        // Colección de posiciones de equipos
        public virtual ICollection<PosicionEquipo> Posiciones { get; set; } = new HashSet<PosicionEquipo>();

        #region Metodos de negocio

        /// <summary>
        /// Obtiene las posiciones ordenadas por criterios deportivos
        /// 1. Puntos DESC
        /// 2. Diferencia de goles DESC
        /// 3. Goles a favor DESC
        /// 4. Nombre alfabético ASC
        /// </summary>
        
        public IEnumerable<PosicionEquipo> ObtenerPosicionesOrdenadas() 
        {
            return Posiciones
                .OrderByDescending(p => p.Puntos)
                .ThenByDescending(p => p.GolesAFavor - p.GolesEnContra) // Diferencia
                .ThenByDescending(p => p.GolesAFavor)
                .ThenBy(p => p.Equipo.Nombre);
        }

        #endregion


    }
}
