
namespace TSL.Core.Application.Dtos.Liga
{
    public class LigaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion {  get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }

        // Propiedades calculadas
        public int CantidadTemporadas { get; set; }
        public string EstadoTexto => Estado ? "Activa" : "Inactiva";
    }
}
