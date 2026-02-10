using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;
using TSL.Core.Domain.Enums;

namespace TSL.Infrastructure.Persistence.Configurations
{
    public class PartidoConfiguration : IEntityTypeConfiguration<Partido>
    {
        public void Configure(EntityTypeBuilder<Partido> builder) 
        {
            builder.ToTable("Partidos");

            builder.HasKey(p => p.Id);

            // Propiedades
            builder.Property(p => p.TemporadaId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.EquipoLocalId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.EquipoVisitanteId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.Fecha)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(p => p.Jornada)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.GolesLocal)
                .HasColumnType("int");

            builder.Property(p => p.GolesVisitante)
                .HasColumnType("int");

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasDefaultValue(EstadoPartido.Programado)
                .HasConversion<byte>();

            // Indices
            builder.HasIndex(p => p.TemporadaId)
                .HasDatabaseName("IX_Partidos_TemporadaId");

            builder.HasIndex(p => p.EquipoLocalId)
                .HasDatabaseName("IX_Partidos_EquipoLocalId");

            builder.HasIndex(p => p.EquipoVisitanteId)
                .HasDatabaseName("IX_Partidos_EquipoVisitanteId");

            builder.HasIndex(p => new { p.TemporadaId, p.Jornada, p.Fecha })
                .HasDatabaseName("IX_Partidos_Temporada_Jornada_Fecha");

        }
    }
}
