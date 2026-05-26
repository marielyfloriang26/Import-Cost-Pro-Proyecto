using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
    {
        public void Configure(EntityTypeBuilder<Moneda> builder)
        {
            builder.ToTable("Monedas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(m => m.CodigoIso)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasIndex(m => m.CodigoIso)
                .IsUnique();

            builder.Property(m => m.Simbolo)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(m => m.EsMonedaLocal)
                .IsRequired();

            builder.Property(m => m.Estado)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
