using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Core.Domain.Enums;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class PartidoRepository : GenericRepository<Partido>, IPartidoRepository
    {
        public PartidoRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<Partido?> GetByIdWithDetallesAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Temporada)
                    .ThenInclude(t => t.Liga)
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Partido>> GetByTemporadaAsync(int temporadaId)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                .Where(p => p.TemporadaId == temporadaId)
                .OrderBy(p => p.Jornada)
                .ThenBy(p => p.Fecha)
                .ToListAsync();
        }

        public async Task<List<Partido>> GetByJornadaAsync(int temporadaId, int jornada)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                .Where(p => p.TemporadaId == temporadaId && p.Jornada == jornada)
                .OrderBy(p => p.Fecha)
                .ToListAsync();
        }

        public async Task<List<Partido>> GetByEquipoAsync(int equipoId)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                .Where(p => p.EquipoLocalId == equipoId || p.EquipoVisitanteId == equipoId)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        public async Task<List<Partido>> GetByEstadoAsync(EstadoPartido estado)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                .Where(p => p.Estado == estado)
                .OrderBy(p => p.Fecha)
                .ToListAsync();
        }

        public async Task<List<Partido>> GetProximosPartidosAsync(int cantidad = 5)
        {
            var ahora = DateTime.UtcNow;

            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                    .ThenInclude(t => t.Liga)
                .Where(p => p.Estado == EstadoPartido.Programado && p.Fecha >= ahora)
                .OrderBy(p => p.Fecha)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<List<Partido>> GetUltimosResultadosAsync(int cantidad = 5)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Include(p => p.Temporada)
                    .ThenInclude(t => t.Liga)
                .Where(p => p.Estado == EstadoPartido.Jugado)
                .OrderByDescending(p => p.Fecha)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<bool> EquipoTienePartidoEnJornadaAsync(
            int equipoId,
            int jornada,
            int temporadaId,
            int? excludePartidoId = null)
        {
            var query = _dbSet.Where(p =>
                p.TemporadaId == temporadaId &&
                p.Jornada == jornada &&
                (p.EquipoLocalId == equipoId || p.EquipoVisitanteId == equipoId)
            );

            if (excludePartidoId.HasValue)
            {
                query = query.Where(p => p.Id != excludePartidoId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<List<Partido>> GetPartidosJugadosAsync(int temporadaId)
        {
            return await _dbSet
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .Where(p => p.TemporadaId == temporadaId && p.Estado == EstadoPartido.Jugado)
                .OrderBy(p => p.Jornada)
                .ThenBy(p => p.Fecha)
                .ToListAsync();
        }


    }
}
