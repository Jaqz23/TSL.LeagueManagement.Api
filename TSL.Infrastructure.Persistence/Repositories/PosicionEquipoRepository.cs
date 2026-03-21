using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class PosicionEquipoRepository : GenericRepository<PosicionEquipo>, IPosicionEquipoRepository
    {
        public PosicionEquipoRepository(ApplicationContext context) : base(context)
        { 
        }

        public async Task<PosicionEquipo?> GetByEquipoAndTablaAsync(int equipoId, int tablaPosicionId)
        {
            return await _dbSet
                .Include(pe => pe.Equipo)
                .Include(pe => pe.TablaPosicion)
                    .ThenInclude(pe => pe.Temporada)
                .FirstOrDefaultAsync(pe => pe.EquipoId == equipoId && pe.TablaPosicionId == tablaPosicionId);
        }

        public async Task<List<PosicionEquipo>> GetPosicionesOrdenadasAsync(int tablaPosicionId)
        {
            return await _dbSet
                .Include(pe => pe.Equipo)
                .Where(pe => pe.TablaPosicionId == tablaPosicionId)
                .OrderByDescending(pe => pe.Puntos)
                .ThenByDescending(pe => pe.GolesAFavor - pe.GolesEnContra) // Diferencia de gol
                .ThenByDescending(pe => pe.GolesAFavor)
                .ThenBy(pe => pe.Equipo.Nombre)
                .ToListAsync();
        }

        public async Task<PosicionEquipo?> GetPosicionActualEquipoAsync(int equipoId, int temporadaId)
        {
            return await _dbSet
                .Include(pe => pe.Equipo)
                .Include(pe => pe.TablaPosicion)
                    .ThenInclude(tp => tp.Temporada)
                .FirstOrDefaultAsync(pe => pe.EquipoId == equipoId && pe.TablaPosicion.TemporadaId == temporadaId);
        }

        public async Task<List<PosicionEquipo>> GetByTablaPosicionIdAsync(int tablaPosicionId) 
        {
            return await _context.PosicionesEquipos
                .Include(p => p.Equipo)
                .Where(p => p.TablaPosicionId == tablaPosicionId) 
                .ToListAsync();
        }

        public async Task RemoveAllByTablaAsync(int tablaPosicionId)
        {
            var posiciones = await _dbSet
                .Where(pe => pe.TablaPosicionId == tablaPosicionId)
                .ToListAsync();

            _dbSet.RemoveRange(posiciones);
        }

        public async Task<List<PosicionEquipo>> GetHistorialByEquipoAsync(int equipoId)
        {
            return await _context.PosicionesEquipos
                .Include(p => p.Equipo)
                .Include(p => p.TablaPosicion)
                    .ThenInclude(t  => t.Temporada)
                        .ThenInclude(temp => temp.Liga)
                .Where(p => p.EquipoId == equipoId)
                .OrderByDescending(p => p.TablaPosicion.Temporada.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistePosicionAsync(int equipoId, int tablaPosicionId)
        {
            return await _context.PosicionesEquipos
            .AnyAsync(p => p.EquipoId == equipoId && p.TablaPosicionId == tablaPosicionId);
        }
    }
}
