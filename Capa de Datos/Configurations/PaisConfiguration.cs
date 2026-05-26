using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class PaisConfiguration : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("Paises");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.CodigoIso)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasIndex(p => p.CodigoIso)
                .IsUnique();

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
