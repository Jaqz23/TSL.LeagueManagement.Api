using Microsoft.Extensions.DependencyInjection;
using TSL.Core.Application.Interfaces.Services;
using TSL.Core.Application.Mappings;
using TSL.Core.Application.Services;

namespace TSL.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services) 
        {
            services.AddAutoMapper(cfg => 
            {
                cfg.AddProfile<GeneralProfile>();
            });

            #region Services

            services.AddScoped<ILigaService, LigaService>();
            services.AddScoped<ITemporadaService, TemporadaService>();
            services.AddScoped<IEquipoService, EquipoService>();
            services.AddScoped<IPosicionEquipoService, PosicionEquipoService>();
            services.AddScoped<ITablaPosicionService, TablaPosicionService>();
            services.AddScoped<IPartidoService, PartidoService>();

            #endregion

        }
    }
}
