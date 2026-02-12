using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private IDbContextTransaction? _transaction;

        // Repositorios (lazy initialization)
        private ILigaRepository? _ligaRepository;
        private ITemporadaRepository? _temporadaRepository;
        private IEquipoRepository? _equipoRepository;
        private IPartidoRepository? _partidoRepository;
        private ITablaPosicionRepository? _tablaPosicionRepository;
        private IPosicionEquipoRepository? _posicionEquipoRepository;

        public UnitOfWork(ApplicationContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Propiedades de Repositorios (Lazy Loading)

        public ILigaRepository LigaRepository 
        {
            get 
            {
                _ligaRepository ??= new LigaRepository(_context);
                return _ligaRepository;
            }
        }

        public ITemporadaRepository TemporadaRepository
        {
            get
            {
                _temporadaRepository ??= new TemporadaRepository(_context);
                return _temporadaRepository;
            }
        }

        public IEquipoRepository EquipoRepository
        {
            get
            {
                _equipoRepository ??= new EquipoRepository(_context);
                return _equipoRepository;
            }
        }

        public IPartidoRepository PartidoRepository
        {
            get
            {
                _partidoRepository ??= new PartidoRepository(_context);
                return _partidoRepository;
            }
        }

        public ITablaPosicionRepository TablaPosicionRepository
        {
            get
            {
                _tablaPosicionRepository ??= new TablaPosicionRepository(_context);
                return _tablaPosicionRepository;
            }
        }


        public IPosicionEquipoRepository PosicionEquipoRepository
        {
            get
            {
                _posicionEquipoRepository ??= new PosicionEquipoRepository(_context);
                return _posicionEquipoRepository;
            }
        }


        #endregion


        #region Metodos de Transaccion

        public async Task<int> SaveChangesAsync() 
        {
            try 
            {
                return await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex) 
            {
                // Manejar conflictos de concurrencia
                throw new InvalidOperationException(
                    "Error de concurrencia: El registro fue modificado por otro usuario.", ex);
            }
            catch(DbUpdateException ex) 
            {
                // Manejar errores de base de datos
                throw new InvalidOperationException(
                    "Error al guardar los cambios en la base de datos.", ex);
            }
        }


        public int SaveChanges() 
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "Error de concurrencia: El registro fue modificado por otro usuario.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "Error al guardar los cambios en la base de datos.", ex);
            }
        }


        public async Task BeginTransactionAsync() 
        {
            if (_transaction != null) 
            {
                throw new InvalidOperationException("Ya existe una transacción activa.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }


        public async Task CommitTransactionAsync() 
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No hay ninguna transacción activa.");
            }

            try // codigo que puede fallar
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch // manejo del error
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally // limpieza obligatoria
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }


        public async Task RollbackTransactionAsync() 
        {
            if(_transaction == null) 
            {
                throw new InvalidOperationException("No hay ninguna transacción activa.");
            }
            try 
            {
                await _transaction.RollbackAsync();
            }
            finally 
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        #endregion


        #region Metodos Adicionales

        public void Reset() 
        {
            _context.ChangeTracker.Clear();
        }

        public bool HasChanges() 
        {
            return _context.ChangeTracker.HasChanges();
        }

        #endregion


        #region Dispose Pattern'

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing) 
        {
            if (!_disposed) 
            {
                if (disposing) 
                {
                    // Liberar la transaccion si existe
                    _transaction?.Dispose();

                    // Liberar el contexto
                    _context?.Dispose();
                }

                _disposed = true;
            }
        }

        // Libera los recursos
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
