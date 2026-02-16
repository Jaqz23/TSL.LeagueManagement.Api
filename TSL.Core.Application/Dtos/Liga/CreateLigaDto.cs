using System.ComponentModel.DataAnnotations;

namespace TSL.Core.Application.Dtos.Liga
{
    public class CreateLigaDto
    {
        [Required(ErrorMessage ="El nombre de la liga es obligatorio")]
        [StringLength(100, MinimumLength =3, ErrorMessage ="El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripcion no puede exceder 200 caracteres")]
        public string ? Descripcion {  get; set; }

        public bool Estado { get; set; } = true;
    }
}
