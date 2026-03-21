using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface IPosicionEquipoRepository : IGenericRepository<PosicionEquipo>
    {
        // Obtiene la posicion de un equipo en una tabla especifica
        Task<PosicionEquipo?> GetByEquipoAndTablaAsync(int equipoId, int tablaPosicionId);

        // Obtiene todas las posiciones de una tabla ordenadas
        Task<List<PosicionEquipo>> GetPosicionesOrdenadasAsync(int tablaPosicionId);

        // Obtiene la posicion de un equipo en la temporada actual
        Task<PosicionEquipo?> GetPosicionActualEquipoAsync(int equipoId, int temporadaId);

        // Obtiene todas las posiciones de equipos de una tabla especifica
        Task<List<PosicionEquipo>> GetByTablaPosicionIdAsync (int tablaPosicionId);

        // Elimina todas las posiciones de una tabla
        Task RemoveAllByTablaAsync(int tablaPosicionId);

        // Obtiene el historial de posiciones de un equipo en todas las temporadas
        Task<List<PosicionEquipo>> GetHistorialByEquipoAsync(int equipoId);

        // Verifica si existe una posición para un equipo en una tabla específica
        Task<bool> ExistePosicionAsync(int equipoId, int tablaPosicionId);

    }
}
