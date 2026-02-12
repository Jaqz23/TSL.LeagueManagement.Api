

namespace TSL.Core.Application.Interfaces.Repositories
{

    // Patron Unit of Work - Coordina el trabajo de multiples repositorios y mantiene una unica transaccion de base de datos
    
    public interface IUnitOfWork : IDisposable
    {
        #region Repositorios

        ILigaRepository LigaRepository { get; }
        ITemporadaRepository TemporadaRepository { get; }
        IEquipoRepository EquipoRepository { get; }
        IPartidoRepository PartidoRepository { get; }
        ITablaPosicionRepository TablaPosicionRepository { get; }
        IPosicionEquipoRepository PosicionEquipoRepository { get; }

        #endregion

        #region Metodos de Transaccion

        // Guarda todos los cambios realizados en el contexto
        Task<int> SaveChangesAsync();

        // Guarda cambios de forma sincrona
        int SaveChanges();

        // Inicia una transacción explicita
        Task BeginTransactionAsync();

        // Confirma la transacción actual
        Task CommitTransactionAsync();

        // Revierte la transaccion actual
        Task RollbackTransactionAsync();

        #endregion

        #region Metodos Adicionales

        // Resetea el contexto descartando todos los cambios no guardados
        void Reset();

        // Verifica si hay cambios pendientes
        bool HasChanges();

        #endregion
    }
}
