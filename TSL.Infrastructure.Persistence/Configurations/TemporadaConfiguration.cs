using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Configurations
{
    public class TemporadaConfiguration : IEntityTypeConfiguration<Temporada>
    {
        public void Configure(EntityTypeBuilder<Temporada> builder) 
        {
            builder.ToTable("Temporadas");

            builder.HasKey(t => t.Id);

            // Propiedades
            builder.Property(t => t.LigaId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.Property(t => t.FechaInicio)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(t => t.FechaFin)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(t => t.Estado)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true);

            // Indices
            builder.HasIndex(t => t.LigaId)
                .HasDatabaseName("IX_Temporadas_LigaId");

            // Relaciones
            // Relacion con Liga (ya configurada en LigaConfiguration)

            // Relacion 1:1 con TablaPosicion
            builder.HasOne(t => t.TablaPosicion)
                .WithOne(tp => tp.Temporada)
                .HasForeignKey<TablaPosicion>(tp => tp.TemporadaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TablasPosiciones_Temporada");

            // Relacion con Partido
            builder.HasMany(t => t.Partidos)
                .WithOne(p => p.Temporada)
                .HasForeignKey(p => p.TemporadaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Partidos_Temporada");

        }
    }
}
