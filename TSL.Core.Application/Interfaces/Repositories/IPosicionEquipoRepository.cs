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

        // Elimina todas las posiciones de una tabla
        Task RemoveAllByTablaAsync(int tablaPosicionId);

    }
}
