using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using TSL.Core.Application.Dtos.Common;
using TSL.Infrastructure.Identity.Context;
using TSL.Infrastructure.Identity.Entities;
using TSL.Infrastructure.Identity.Seeds;
using TSL.Infrastructure.Identity.Settings;

namespace TSL.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration) 
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

            #region Configuración de Identity

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                
                options.Password.RequireDigit = true;              
                options.Password.RequireLowercase = true;          
                options.Password.RequireUppercase = true;          
                options.Password.RequireNonAlphanumeric = true;    
                options.Password.RequiredLength = 8;               
                options.Password.RequiredUniqueChars = 1;          

                options.User.RequireUniqueEmail = true;            

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  
                options.Lockout.MaxFailedAccessAttempts = 5;                       
                options.Lockout.AllowedForNewUsers = true;                         

                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            #endregion

            #region Configuración JWT Authentication

            var jwtSettings = configuration.GetSection("JWT").Get<JWTSettings>();

            if (jwtSettings == null || !jwtSettings.IsValid()) 
            {
                var errors = jwtSettings?.GetValidationsErros() ?? new List<string> { "JWTSettings no encontrado en appsettings.json" };
                throw new InvalidOperationException(
                    $"Configuración JWT inválida:\n{string.Join("\n", errors)}"
                );
            }

            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;


                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewInMinutes)
                };

                options.Events = new JwtBearerEvents 
                {
                    OnAuthenticationFailed = context => 
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }

                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new BaseResponseDto<object>
                        {
                            Success = false,
                            Message = "Error de autenticación",
                            Errors = new List<string> { "Token inválido o expirado" }
                        };

                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));

                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new BaseResponseDto<object>
                        {
                            Success = false,
                            Message = "No autorizado",
                            Errors = new List<string> { "Se requiere autenticación para acceder a este recurso" }
                        };

                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new BaseResponseDto<object>
                        {
                            Success = false,
                            Message = "Acceso denegado",
                            Errors = new List<string> { "No tiene permisos para acceder a este recurso" }
                        };

                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }

                };

            });


            #endregion

            #region Registrar Servicios de Identity

            //services.AddTransient<IAccountService, AccountService>();

            #endregion

            Console.WriteLine("Identity Infrastructure configurado correctamente");

        }

        //Ejecuta los seeds de datos iniciales
        public static async Task RunIdentitySeeds (this IServiceProvider serviceProvider) 
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DefaultRoles.SeedAsync(userManager, roleManager);

                await DefaultSuperAdminUser.SeedAsync(userManager, roleManager);
                await DefaultAdminUser.SeedAsync(userManager, roleManager);
                await DefaultBasicUser.SeedAsync(userManager, roleManager);

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error ejecutando seeds: {ex.Message}");
            }
        }

    }
}
