namespace TSL.WebApi.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseSwaggerExtension(this IApplicationBuilder app, IWebHostEnvironment env) 
        {
            // Solo habilitar Swagger en Development y Staging
            if (env.IsDevelopment() || env.IsStaging()) 
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => 
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TSL API v1");

                    options.RoutePrefix = "swagger";

                    options.DocumentTitle = "TSL API";
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    options.DefaultModelsExpandDepth(-1); // Ocultar modelos por defecto
                    options.DisplayRequestDuration();

                });
            }

            return app;

        }

    }
}
