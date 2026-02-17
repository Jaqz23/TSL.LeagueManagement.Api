using Microsoft.Extensions.DependencyInjection;
using TSL.Core.Application.Mappings;

namespace TSL.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services) 
        {
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<GeneralProfile>();
            });

            #region Services

            //To do

            #endregion

        }
    }
}
