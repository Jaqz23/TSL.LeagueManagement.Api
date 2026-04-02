using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TSL.Infrastructure.Identity.Entities;

namespace TSL.Infrastructure.Identity.Context
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options): base (options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");

            #region Configuración de nombres de tablas de Identity

            builder.Entity<ApplicationUser>(entity => 
            {
                entity.ToTable(name: "Users");

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                // Relacion con RefreshTokens
                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(rt => rt.ApplicationUser)
                    .HasForeignKey(rt => rt.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable(name: "UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable(name: "RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable(name: "UserTokens");
            });

            // Configuracion de RefreshTokens
            builder.Entity<RefreshToken>(entity => 
            {
                entity.ToTable(name: "RefreshTokens");

                // Indice para busquedas rapidas por token
                entity.HasIndex(rt => rt.Token);

                // Indice para busqueda por usuario
                entity.HasIndex(rt => rt.ApplicationUserId);

                // Configuracion de propiedades
                entity.Property(rt => rt.Token)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(rt => rt.ApplicationUserId)
                    .IsRequired();

            });

            #endregion

        }
    }
}
