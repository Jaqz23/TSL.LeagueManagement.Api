using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class EquipoRepository : GenericRepository<Equipo>, IEquipoRepository
    {
        public EquipoRepository(ApplicationContext context) : base(context)
        {
        }


        #region Override para incluir los partidos

        public override async Task<Equipo?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.PartidosLocal)
                .Include(e => e.PartidosVisitante)
                .FirstOrDefaultAsync(e => e.Id == id);
        }


        public override async Task<List<Equipo>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.PartidosLocal)
                .Include(e => e.PartidosVisitante)
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        #endregion


        public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
        {
            var query = _dbSet.Where(e => e.Nombre.ToLower() == nombre.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Equipo?> GetByIdWithPartidosAsync(int id)
        {
            return await _dbSet
                .Include(e => e.PartidosLocal)
                    .ThenInclude(p => p.EquipoVisitante)
                .Include(e => e.PartidosLocal)
                    .ThenInclude(p => p.Temporada)
                .Include(e => e.PartidosVisitante)
                    .ThenInclude(p => p.EquipoLocal)
                .Include(e => e.PartidosVisitante)
                     .ThenInclude(p => p.Temporada)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Equipo?> GetByIdWithPosicionesAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Posiciones)
                    .ThenInclude(p => p.TablaPosicion)
                        .ThenInclude(tp => tp.Temporada)
                .FirstOrDefaultAsync (e => e.Id == id);
        }

        public async Task<bool> TienePartidosAsync(int equipoId)
        {
            return await _context.Partidos
                .AnyAsync(p => p.EquipoLocalId == equipoId || p.EquipoVisitanteId == equipoId);
        }

        public async Task<List<Equipo>> GetByCiudadAsync(string ciudad)
        {
            return await _dbSet
                .Where(e => e.Ciudad != null && e.Ciudad.ToLower() == ciudad.ToLower()) 
                .OrderBy(e => e.Nombre)
                .ToListAsync();

        }

    }
}
