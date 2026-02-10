using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Configurations
{
    // Configuracion de la entidad Liga
    public class LigaConfiguration : IEntityTypeConfiguration<Liga>
    {
        public void Configure(EntityTypeBuilder<Liga> builder) 
        {
            // Nombre de la tabla
            builder.ToTable("Ligas");

            // Clave primaria
            builder.HasKey(l => l.Id);

            // Propiedades
            builder.Property(l => l.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("Varchar(100)");

            builder.Property(l => l.Descripcion)
                .HasMaxLength(200)
                .HasColumnType("varchar(200)");

            builder.Property(l => l.FechaCreacion)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(l => l.Estado)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true);

            // Indices
            builder.HasIndex(l => l.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Ligas_Nombre");

            // Relaciones
            builder.HasMany(l => l.Temporadas)
                .WithOne(t => t.Liga)
                .HasForeignKey(t => t.LigaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Temporadas_Ligas");

        }
    }
}
