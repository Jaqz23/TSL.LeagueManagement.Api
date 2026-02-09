using TSL.Core.Domain.Common;

namespace TSL.Core.Domain.Entities
{
    public class Temporada : BaseEntity
    {
        public int LigaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        // /// Estado: true = Activa, false = Finalizada
        public bool Estado { get; set; } = true;


        // Navigation property

        // Liga a la que pertenece esta temporada
        public virtual Liga Liga { get; set; } = null!;

        // Colección de partidos de esta temporada
        public virtual ICollection<Partido> Partidos { get; set; } = new HashSet<Partido>();

        // Tabla de posicion de esta Temporada
        public virtual TablaPosicion? TablaPosicion { get; set; }

        #region Metodos de negocio

        // Valida si las fechas son coherentes
        public bool TieneFechasValidas() 
        {
            return FechaInicio < FechaFin;
        }

        // Valida si una fecha esta dentro del rango de la temporada
        public bool FechaEstaEnRango(DateTime fecha) 
        {
            return fecha >= FechaInicio && fecha <= FechaFin;
        }

        // Finaliza la temporada
        public void Finalizar() 
        {
            Estado = false;
        }

        // Valida si puede ser eliminada
        // Regla: No se puede eliminar si tiene partidos
        public bool PuedeSerEliminada()
        {
            return !Partidos.Any();
        }

        #endregion
    }
}
