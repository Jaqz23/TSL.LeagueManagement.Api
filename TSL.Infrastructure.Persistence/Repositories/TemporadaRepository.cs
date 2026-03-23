using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class TemporadaRepository : GenericRepository<Temporada>, ITemporadaRepository
    {
        public TemporadaRepository(ApplicationContext context) : base(context)
        {
        }


        #region Override para incluir los partidos

        public override async Task<Temporada?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Liga)
                .Include(t => t.Partidos)
                .FirstOrDefaultAsync(t => t.Id == id);
        }


        public override async Task<List<Temporada>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.Liga)
                .Include(t => t.Partidos)
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }


        #endregion

        public async Task<Temporada?> GetByIdWithDetallesAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Liga)
                .Include(t => t.Partidos)
                    .ThenInclude(p => p.EquipoLocal)
                .Include(t => t.Partidos)
                    .ThenInclude(p => p.EquipoVisitante)
                .Include(t => t.TablaPosicion)
                    .ThenInclude(tp => tp!.Posiciones)
                        .ThenInclude(pe => pe.Equipo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Temporada?> GetTemporadaActivaByLigaAsync(int ligaId)
        {
            return await _dbSet
                .Include(t => t.Liga)
                .Include (t => t.Partidos)
                .Include(t => t.TablaPosicion)
                .FirstOrDefaultAsync(t => t.LigaId == ligaId && t.Estado == true);
        }

        public async Task<List<Temporada>> GetByLigaIdAsync(int ligaId)
        {
            return await _dbSet 
                .Include(t => t.Liga)
                .Include(t => t.Partidos)
                .Where(t => t.LigaId == ligaId)
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<bool> TienePartidosAsync(int temporadaId)
        {
            return await _context.Partidos
                .AnyAsync(P => P.TemporadaId == temporadaId);
        }

        public async Task<List<Temporada>> GetTemporadasActivasAsync()
        {
            return await _dbSet
                .Include(t => t.Liga)
                .Include(t => t.Partidos)
                .Where(t => t.Estado == true)
                .OrderByDescending(t =>t.FechaInicio)
                .ToListAsync();
        }

        public async Task<bool> ExisteOtraTemporadaActivaAsync(int ligaId, int? excludeId = null)
        {
            var query = _dbSet.Where(t => t.LigaId == ligaId && t.Estado == true);

            if (excludeId.HasValue) 
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
