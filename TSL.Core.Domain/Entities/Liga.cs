using TSL.Core.Domain.Common;

namespace TSL.Core.Domain.Entities
{
    public class Liga : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;

        public string ? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Estado { get; set; } = true;

       
        // Navigation property

        //Coleccion de temporadas asociadas a esta liga
        public virtual ICollection<Temporada> Temporadas { get; set; } = new HashSet<Temporada>();

        #region Metodos de negocio
        
        // Valida si la liga puede ser eliminada
        public bool PuedeSerEliminada() 
        {
            return !Temporadas.Any();
        }

        // Activa la Liga
        public void Activar() 
        {
            Estado = true;
        }


        // Desactiva la Liga
        public void Desactivar() 
        {
            Estado = false;
        }

        #endregion

    }
}
