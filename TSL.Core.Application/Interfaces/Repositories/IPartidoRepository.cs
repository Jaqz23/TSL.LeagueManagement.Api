using TSL.Core.Domain.Entities;
using TSL.Core.Domain.Enums;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface IPartidoRepository : IGenericRepository<Partido>
    {
        // Obtiene un partido con todos sus datos relacionados
        Task<Partido?> GetByIdWithDetallesAsync(int id);

        // Obtiene todos los partidos de una temporada
        Task<List<Partido>> GetByTemporadaAsync(int temporadaId);

        // Obtiene partidos de una jornada especifica
        Task<List<Partido>> GetByJornadaAsync(int temporadaId, int jornada);

        // Obtiene partidos de un equipo (local + visitante)
        Task<List<Partido>> GetByEquipoAsync(int equipoId);

        // Obtiene partidos por estado
        Task<List<Partido>> GetByEstadoAsync(EstadoPartido estado);

        // Obtiene proximos partidos programados
        Task<List<Partido>> GetProximosPartidosAsync(int cantidad = 5);

        // Obtiene ultimos partidos jugados
        Task<List<Partido>> GetUltimosResultadosAsync(int cantidad = 5);

        // Verifica si un equipo tiene partido en una jornada especifica
        Task<bool> EquipoTienePartidoEnJornadaAsync(
            int equipoId,
            int jornada,
            int temporadaId,
            int? excludePartidoId = null);

        // Obtiene partidos jugados de una temporada
        Task<List<Partido>> GetPartidosJugadosAsync(int temporadaId);

    }
}
