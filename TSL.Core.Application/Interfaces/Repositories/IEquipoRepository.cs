using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface IEquipoRepository : IGenericRepository<Equipo>
    {
        // Verifica si existe un equipo con el nombre especificado
        Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);

        // Obtiene un equipo con todos sus partidos
        Task<Equipo?> GetByIdWithPartidosAsync(int id);

        // Obtiene un equipo con sus posiciones en tablas
        Task<Equipo?> GetByIdWithPosicionesAsync(int id);

        // Verifica si el equipo tiene partidos asociados
        Task<bool> TienePartidosAsync(int equipoId);

        // Obtiene equipos por ciudad
        Task<List<Equipo>> GetByCiudadAsync(string ciudad);

    }
}
