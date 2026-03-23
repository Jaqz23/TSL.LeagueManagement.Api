using Microsoft.EntityFrameworkCore;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Core.Domain.Entities;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence.Repositories
{
    public class LigaRepository : GenericRepository<Liga> , ILigaRepository
    {
        public LigaRepository(ApplicationContext context) : base(context)
        {
        }

        #region Override para incluir las temporadas

        public override async Task<Liga?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Temporadas)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public override async Task<List<Liga>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Temporadas)
                .OrderByDescending(l => l.FechaCreacion)
                .ToListAsync();
        }

        #endregion

        public async Task<Liga?> GetByIdWithTemporadasAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Temporadas)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
        {
            var query = _dbSet.Where(l => l.Nombre.ToLower() == nombre.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<List<Liga>> GetLigasActivasAsync()
        {
            return await _dbSet
                .Where(l => l.Estado == true)
                .OrderBy(l => l.Nombre)
                .ToListAsync();
        }

        public async Task<List<Liga>> GetLigasConTemporadasActivasAsync()
        {
            return await _dbSet
                .Include(l => l.Temporadas.Where(t => t.Estado == true))
                .Where(l => l.Estado == true)
                .OrderBy(l => l.Nombre)
                .ToListAsync();
        }
    }
}
