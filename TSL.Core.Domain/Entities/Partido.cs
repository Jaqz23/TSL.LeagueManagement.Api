using TSL.Core.Domain.Common;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Domain.Entities
{
    public class Partido : BaseEntity
    {
        public int TemporadaId { get; set; }

        public int EquipoLocalId { get; set; }

        public int EquipoVisitanteId { get; set; }

        public DateTime Fecha { get; set; }

        public int Jornada { get; set; }

        public int? GolesLocal { get; set; }

        public int? GolesVisitante { get; set; }

        public EstadoPartido Estado { get; set; } = EstadoPartido.Programado;


        // Navigation property

        // Temporada a la que pertenece
        public virtual Temporada Temporada { get; set; } = null!;

        public virtual Equipo EquipoLocal { get; set; } = null!;

        public virtual Equipo EquipoVisitante { get; set; } = null!;

        #region Metodos de negocio

        // Valida que los equipos sean diferentes
        public bool TieneEquiposDiferentes()
        {
            return EquipoLocalId != EquipoVisitanteId;
        }

        // Registra el resultado del partido
        public void RegistrarResultado(int golesLocal, int golesVisitante) 
        {
            if (Estado == EstadoPartido.Jugado) 
                throw new InvalidOperationException("No se puede cambiar el resultado de un partido ya jugado");

            GolesLocal = golesLocal;
            GolesVisitante = golesVisitante;
            Estado = EstadoPartido.Jugado;
            
        }


        // Valida si puede registrar resultado
        public bool PuedeRegistrarResultado() 
        {
            return Estado != EstadoPartido.Jugado;
        }

        // Obtiene el ID del equipo ganador (null si es empate)
        public int? ObtenerGanadorId() 
        {
            if(Estado != EstadoPartido.Jugado || !GolesLocal.HasValue || !GolesVisitante.HasValue)
                return null;

            if (GolesLocal > GolesVisitante)
                return EquipoLocalId;

            if(GolesVisitante > GolesLocal)
                return EquipoVisitanteId;

            return null; // empate
        }


        // Valida si fue empate
        public bool FueEmpate()
        {
            return Estado == EstadoPartido.Jugado &&
                   GolesLocal.HasValue &&
                   GolesVisitante.HasValue &&
                   GolesLocal == GolesVisitante;
        }

        #endregion

    }
}
