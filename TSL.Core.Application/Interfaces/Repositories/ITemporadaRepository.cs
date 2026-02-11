using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface ITemporadaRepository : IGenericRepository<Temporada>
    {
        // Obtiene una temporada con todos sus datos relacionados
        Task<Temporada?> GetByIdWithDetallesAsync(int id);

        // Obtiene la temporada activa de una liga
        Task<Temporada?> GetTemporadaActivaByLigaAsync(int ligaId);

        // Obtiene todas las temporadas de una liga
        Task<List<Temporada>> GetByLigaIdAsync(int ligaId);

        // Verifica si la temporada tiene partidos registrados
        Task<bool> TienePartidosAsync(int temporadaId);

        // Obtiene temporadas activas
        Task<List<Temporada>> GetTemporadasActivasAsync();

        // Verifica si existe otra temporada activa en la misma liga
        Task<bool> ExisteOtraTemporadaActivaAsync(int ligaId, int? excludeId = null);
    }
}
