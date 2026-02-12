using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TSL.Core.Application.Interfaces;
using TSL.Core.Application.Interfaces.Repositories;
using TSL.Infrastructure.Persistence.Contexts;
using TSL.Infrastructure.Persistence.Repositories;

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

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // TODO: Agregar aqui los repositorios

            #endregion

            #region Unit of Work

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion

        }
    }
}
