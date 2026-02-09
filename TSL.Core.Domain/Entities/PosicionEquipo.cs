using TSL.Core.Domain.Common;

namespace TSL.Core.Domain.Entities
{
    public class PosicionEquipo : BaseEntity
    {
        public int TablaPosicionId { get; set; }

        public int EquipoId { get; set; }

        public int PartidosJugados { get; set; }

        public int Victorias { get; set; }

        public int Empates { get; set; }

        public int Derrotas { get; set; }

        public int GolesAFavor { get; set; }

        public int GolesEnContra { get; set; }

        public int Puntos { get; set; }

        // Navigation property

        // Tabla de posición a la que pertenece
        public virtual TablaPosicion TablaPosicion { get; set; } = null!;

        // Equipo asociado
        public virtual Equipo Equipo { get; set; } = null!;


        #region Metodos de negocio

        // Registra una victoria
        // Regla: Victoria = 3 puntos
        public void RegistrarVictoria(int golesFavor, int golesContra) 
        {
            PartidosJugados++;
            Victorias++;
            GolesAFavor += golesFavor;
            GolesEnContra += golesContra;
            Puntos += 3;
        }

        // Registra un empate
        // Regla: Empate = 1 punto
        public void RegistrarEmpate(int goles) 
        {
            PartidosJugados++;
            Empates++;
            GolesAFavor += goles;
            GolesEnContra += goles;
            Puntos += 1;

        }

        // Registra una derrota
        // Regla: Derrota = 0 puntos
        public void RegistrarDerrota(int golesFavor, int golesContra) 
        {
            PartidosJugados++;
            Derrotas++;
            GolesAFavor += golesFavor;
            GolesEnContra += golesContra;
        }


        // Reinicia todas las estadísticas a cero
        public void Reiniciar() 
        {
            PartidosJugados = 0;
            Victorias = 0;
            Empates = 0;
            Derrotas = 0;
            GolesAFavor = 0;
            GolesEnContra = 0;
            Puntos = 0;
        }

        #endregion

    }
}
