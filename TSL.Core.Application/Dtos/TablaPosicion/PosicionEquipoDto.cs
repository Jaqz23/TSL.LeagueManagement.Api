
namespace TSL.Core.Application.Dtos.TablaPosicion
{
    public class PosicionEquipoDto
    {
        public int Id { get; set; }

        // Informacion del equipo
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public string? EscudoEquipo { get; set; }

        //Estadisticas
        public int Posicion { get; set; } // Calculada al ordenar
        public int PartidosJugados { get; set; }
        public int Victorias { get; set; }
        public int Empates { get; set; }
        public int Derrotas { get; set; }
        public int GolesAFavor { get; set; }
        public int GolesEnContra { get; set; }
        public int DiferenciaGoles => GolesAFavor - GolesEnContra;
        public int Puntos { get; set; }

        // Propiedades adicionales
        public string FormatoAbreviado => $"PJ: {PartidosJugados} | V: {Victorias} | E: {Empates} | D: {Derrotas}";

    }
}
