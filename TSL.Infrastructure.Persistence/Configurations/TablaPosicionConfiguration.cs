using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Configurations
{
    public class TablaPosicionConfiguration : IEntityTypeConfiguration<TablaPosicion>
    {
        public void Configure(EntityTypeBuilder<TablaPosicion> builder) 
        {
            builder.ToTable("TablasPosiciones");

            builder.HasKey(tp => tp.Id);

            // Propiedades
            builder.Property(tp => tp.TemporadaId)
                .IsRequired()
                .HasColumnType("int");

            // Indices
            builder.HasIndex(tp => tp.TemporadaId)
                .IsUnique()
                .HasDatabaseName("IX_TablasPosiciones_TemporadaId");

            // Relaciones
            // Relacion 1:1 con Temporada (ya configurada en TemporadaConfiguration)
            // Relación con PosicionEquipo
            builder.HasMany(tp => tp.Posiciones)
                .WithOne(pe => pe.TablaPosicion)
                .HasForeignKey(pe => pe.TablaPosicionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PosicionesEquipos_TablaPosicion");

        }
    }
}
