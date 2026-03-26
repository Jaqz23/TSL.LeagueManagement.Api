
namespace TSL.Core.Application.Dtos.Account
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
    }
}
