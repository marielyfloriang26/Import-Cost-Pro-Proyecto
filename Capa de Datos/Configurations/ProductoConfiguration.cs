using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.CodigoReferencia)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.CodigoReferencia)
                .IsUnique();

            builder.Property(p => p.PesoUnitario)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.Largo)
                .HasPrecision(10, 2);

            builder.Property(p => p.Ancho)
                .HasPrecision(10, 2);

            builder.Property(p => p.Alto)
                .HasPrecision(10, 2);

            builder.Property(p => p.UnidadMedida)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Descripcion)
                .HasMaxLength(250);

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(p => p.PaisOrigen)
                .WithMany(pa => pa.Productos)
                .HasForeignKey(p => p.PaisOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
