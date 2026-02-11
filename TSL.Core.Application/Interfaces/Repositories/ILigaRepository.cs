using TSL.Core.Domain.Entities;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface ILigaRepository : IGenericRepository<Liga>
    {
        // Obtiene una liga con sus temporadas incluidas
        Task<Liga?> GetByIdWithTemporadasAsync(int id);

        // Verifica si existe una liga con el nombre especificado
        Task<bool> ExisteNombreAsync (string nombre, int? excludeId = null);

        // Obtiene todas las ligas activas
        Task<List<Liga>> GetLigasActivasAsync();

        // Obtiene ligas con sus temporadas activas
        Task<List<Liga>> GetLigasConTemporadasActivasAsync();

    }
}
