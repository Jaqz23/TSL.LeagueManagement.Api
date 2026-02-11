using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface ITablaPosicionRepository : IGenericRepository<TablaPosicion>
    {
        // Obtiene la tabla de posiciones de una temporada
        Task<TablaPosicion?> GetByTemporadaIdAsync(int temporadaId);

        // Obtiene la tabla con todas las posiciones ordenadas
        Task<TablaPosicion?> GetByTemporadaIdWithPosicionesAsync(int temporadaId);

        // Verifica si existe tabla de posiciones para una temporada
        Task<bool> ExisteParaTemporadaAsync(int temporadaId);
    }
}
