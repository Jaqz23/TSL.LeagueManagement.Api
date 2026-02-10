using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TSL.Core.Domain.Entities;

namespace TSL.Infrastructure.Persistence.Configurations
{
    public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
    {
        public void Configure(EntityTypeBuilder<Equipo> builder) 
        {
            builder.ToTable("Equipos");

            builder.HasKey(e => e.Id);

            // Propiedades
            builder.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.Ciudad)
                .HasMaxLength(50)
                .HasColumnType("varchar(50)");

            builder.Property(e => e.Escudo)
                .HasMaxLength(500)
                .HasColumnType("varchar(500)");

            builder.Property(e => e.FechaRegistro)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            // Indices
            builder.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Equipos_Nombre");

            // Relaciones con Partidos (Local)
            builder.HasMany(e => e.PartidosLocal)
                .WithOne(p => p.EquipoLocal)
                .HasForeignKey(p => p.EquipoLocalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Partidos_EquipoLocal");

            // Relaciones con Partidos (Visitante)
            builder.HasMany(e => e.PartidosVisitante)
                .WithOne(p => p.EquipoVisitante)
                .HasForeignKey(p => p.EquipoVisitanteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Partidos_EquipoVisitante");

            // Relacion con PosicionEquipo
            builder.HasMany(e => e.Posiciones)
                .WithOne(pe => pe.Equipo)
                .HasForeignKey(pe => pe.EquipoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PosicionesEquipos_Equipo");

        }
    }
}
