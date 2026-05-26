using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class CategoriaArancelariaConfiguration : IEntityTypeConfiguration<CategoriaArancelaria>
    {
        public void Configure(EntityTypeBuilder<CategoriaArancelaria> builder)
        {
            builder.ToTable("CategoriasArancelarias");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.Codigo)
                .IsUnique();

            builder.Property(c => c.Descripcion)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.PorcentajeArancel)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(c => c.AplicaItbis)
                .IsRequired();

            builder.Property(c => c.AplicaSelectivo)
                .IsRequired();

            builder.Property(c => c.PorcentajeSelectivo)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
