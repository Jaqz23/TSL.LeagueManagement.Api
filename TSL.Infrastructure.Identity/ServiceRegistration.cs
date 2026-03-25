using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TSL.Infrastructure.Identity.Context;

namespace TSL.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            #region DbContext de Identity

            var connectionString = configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(
                connectionString, 
                m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)
                )
            );

            #endregion

            return services;
        }
    }
}
