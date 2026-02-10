using Microsoft.EntityFrameworkCore;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }


        #region DbSets

        public DbSet<Liga> Ligas { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<TablaPosicion> TablasPosiciones { get; set; }
        public DbSet<PosicionEquipo> PosicionesEquipos { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas las configuraciones del ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }

    }
}
