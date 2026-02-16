
namespace TSL.Core.Application.Dtos.TablaPosicion
{
    public class TablaPosicionDto
    {
        public int Id { get; set; }
        public int TemporadaId { get; set; }
        public string NombreTemporada { get; set; } = string.Empty;
        public string NombreLiga { get; set; } = string.Empty;

        // Lista de posiciones ordenadas
        public List<PosicionEquipoDto> Posiciones { get; set; } = new();

    }
}
