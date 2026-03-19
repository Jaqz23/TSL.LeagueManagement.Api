using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Dtos.Partido
{
    public class PartidoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int Jornada { get; set; }
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public EstadoPartido Estado { get; set; }

        // Relacion con Temporada
        public int TemporadaId { get; set; }
        public string NombreTemporada { get; set; } = string.Empty;


        // Equipo Local
        public int EquipoLocalId { get; set; }
        public string NombreEquipoLocal { get; set; } = string.Empty;
        public string? EscudoEquipoLocal { get; set; }

        // Equipo Visitante
        public int EquipoVisitanteId { get; set; }
        public string NombreEquipoVisitante { get; set; } = string.Empty;
        public string? EscudoEquipoVisitante { get; set; }


        // Propiedades calculadas
        public string EstadoTexto => Estado switch
        {
            EstadoPartido.Programado => "Programado",
            EstadoPartido.Jugado => "Jugado",
            _ => "Desconocido"
        };

        public string ResultadoTexto
        {
            get
            {
                if (Estado != EstadoPartido.Jugado || !GolesLocal.HasValue || !GolesVisitante.HasValue)
                    return "Sin resultado";

                return $"{GolesLocal} - {GolesVisitante}";
            }
        }

    }
}
