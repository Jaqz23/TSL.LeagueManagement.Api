using Asp.Versioning;
using Microsoft.OpenApi.Models;


namespace TSL.WebApi.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));

                // Info de la API V1
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TSL API - The Sunday League",
                    Description = "API RESTful para gestión de ligas de fútbol amateur. Incluye administración de ligas, temporadas, equipos, partidos y tablas de posiciones.",
                    Contact = new OpenApiContact
                    {
                        Name = "Gustavo Jáquez Rosa",
                        Email = "gustavojaquezr23@gmail.com",
                        Url = new Uri("https://github.com/Jaqz23")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }

                });


                options.DescribeAllParametersInCamelCase();


                // Configuracion de JWT Bearer Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Ingrese su token JWT en el formato: Bearer { token }\n\nEjemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Mapeo de Enums como strings en Swagger UI
                options.UseInlineDefinitionsForEnums();

            });

            return services;

        }


        // Configuracion del versionado de la API, version por defecto v1.0
        public static IServiceCollection AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                
                config.DefaultApiVersion = new ApiVersion(1, 0);

                config.AssumeDefaultVersionWhenUnspecified = true;

                config.ReportApiVersions = true;

                // Leer version desde URL
            }).AddApiExplorer(options =>
            {
                // Formato de version en Swagger: 'v'major[.minor]
                options.GroupNameFormat = "'v'VVV";

                // Sustituir version en rutas
                options.SubstituteApiVersionInUrl = true;
            });

            return services;

        }

        // Configuracion del CORS para permitir acceso
        public static IServiceCollection AddCorsExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });


                options.AddPolicy("ProductionPolicy", builder =>
                {
                    // Obtener origenes permitidos desde appsettings.json
                    var allowdOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                        ?? new[] { "https://localhost:4200" };

                    builder.WithMethods(allowdOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();

                });

            });

            return services;

        }


        // Configurar controladores con opciones especificas
        public static IServiceCollection AddControllersExtension(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            })
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

                // Escribir JSON indentado
                options.JsonSerializerOptions.WriteIndented = true;

            });

            return services;

        }

    }

}
