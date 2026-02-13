using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class TablaPosicionRepository : GenericRepository<TablaPosicion>, ITablaPosicionRepository
    {
        public TablaPosicionRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<TablaPosicion?> GetByTemporadaIdAsync(int temporadaId)
        {
            return await _dbSet
                .Include(tp => tp.Temporada)
                .FirstOrDefaultAsync(tp => tp.TemporadaId == temporadaId);
        }

        public async Task<TablaPosicion?> GetByTemporadaIdWithPosicionesAsync(int temporadaId)
        {
            return await _dbSet
                .Include(tp => tp.Temporada)
                    .ThenInclude(t => t.Liga)
                .Include(tp => tp.Posiciones.OrderByDescending(p => p.Puntos)
                    .ThenByDescending(p => p.GolesAFavor - p.GolesEnContra)
                    .ThenByDescending(p => p.GolesAFavor))
                    .ThenInclude(p => p.Equipo)
                .FirstOrDefaultAsync(tp => tp.TemporadaId == temporadaId);
        }

        public async Task<bool> ExisteParaTemporadaAsync(int temporadaId)
        {
            return await _dbSet.AnyAsync(tp => tp.TemporadaId == temporadaId);
        }

    }
}
