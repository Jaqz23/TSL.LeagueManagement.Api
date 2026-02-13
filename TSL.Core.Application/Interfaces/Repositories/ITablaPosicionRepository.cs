using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface ITablaPosicionRepository : IGenericRepository<TablaPosicion>
    {
        // Obtiene la tabla de posiciones de una temporada
        Task<TablaPosicion?> GetByTemporadaIdAsync(int temporadaId);

        // Obtiene la tabla con todas las posiciones ordenadas
        // 1. Puntos DESC
        // 2. Diferencia de goles DESC
        // 3. Goles a favor DESC
        // 4. Nombre alfabético ASC
        Task<TablaPosicion?> GetByTemporadaIdWithPosicionesAsync(int temporadaId);

        // Verifica si existe tabla de posiciones para una temporada
        Task<bool> ExisteParaTemporadaAsync(int temporadaId);
    }
}
