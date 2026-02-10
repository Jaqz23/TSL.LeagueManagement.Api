using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Configurations
{
    public class PosicionEquipoConfiguration : IEntityTypeConfiguration<PosicionEquipo>
    {
        public void Configure(EntityTypeBuilder<PosicionEquipo> builder) 
        {
            builder.ToTable("PosicionesEquipos");

            builder.HasKey(pe => pe.Id);

            // Propiedades
            builder.Property(pe => pe.TablaPosicionId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(pe => pe.EquipoId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(pe => pe.PartidosJugados)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.Victorias)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.Empates)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.Derrotas)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.GolesAFavor)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.GolesEnContra)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            builder.Property(pe => pe.Puntos)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);

            // Indices
            builder.HasIndex(pe => pe.TablaPosicionId)
                .HasDatabaseName("IX_PosicionesEquipos_TablaPosicionId");

            builder.HasIndex(pe => pe.EquipoId)
                .HasDatabaseName("IX_PosicionesEquipos_EquipoId");

            builder.HasIndex(pe => new { pe.TablaPosicionId, pe.EquipoId })
                .IsUnique()
                .HasDatabaseName("IX_PosicionesEquipos_Tabla_Equipo");

            // Relaciones (ya configuradas en otras entidades)

        }
    }
}
