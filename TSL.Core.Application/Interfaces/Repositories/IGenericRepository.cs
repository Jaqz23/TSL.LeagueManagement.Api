using System.Linq.Expressions;

namespace TSL.Core.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        #region Consultas

        Task<List<TEntity>> GetAllAsync();

        // Obtiene todas las entidades con seguimiento de cambios deshabilitado (AsNoTracking)
        Task<List<TEntity>> GetAllAsNoTrackingAsync();

        // Obtiene una entidad por su ID
        Task<TEntity?> GetByIdAsync(int id);

        // Busca entidades que cumplan con un predicado
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);

        // Busca entidades con inclusion de propiedades relacionadas
        Task<List<TEntity>> FindWithIncludeAsync(
            Expression<Func<TEntity, bool>>filter,
            params Expression<Func<TEntity, object>>[] includeProperties);

        // Verifica si existe alguna entidad que cumpla con el predicado
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);

        // Cuenta cuantas entidades cumplen con el predicado
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);

        // Obtiene el primer elemento que cumple con el filtro o null
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

        #endregion


        #region Comandos

        Task AddAsync(TEntity entity);

        // Agrega multiples entidades
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        // Actualiza multiples entidades
        void UpdateRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        // Elimina multiples entidades
        void RemoveRange(IEnumerable<TEntity> entities);

        #endregion

        // Nota: Las operaciones que solo modifican el estado de la entidad en memoria son sincronicas, por eso no usan Task

    }
}
