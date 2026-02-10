using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TSL.Infrastructure.Persistence.Contexts;

namespace TSL.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            #region Contexts

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(
                connectionString,
                m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)
                )
            );

            #endregion


            #region Repositories
            // TODO: Agregar aqui los repositorios
            #endregion

        }
    }
}
