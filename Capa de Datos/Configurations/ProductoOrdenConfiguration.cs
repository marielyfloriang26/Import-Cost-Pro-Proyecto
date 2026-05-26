using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class ProductoOrdenConfiguration : IEntityTypeConfiguration<ProductoOrden>
    {
        public void Configure(EntityTypeBuilder<ProductoOrden> builder)
        {
            builder.ToTable("ProductosOrden");

            builder.HasKey(po => po.Id);

            builder.Property(po => po.Cantidad)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(po => po.PrecioUnitarioFob)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(po => po.MargenDeseado)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.HasOne(po => po.Orden)
                .WithMany(o => o.ProductosOrden)
                .HasForeignKey(po => po.OrdenId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(po => po.Producto)
                .WithMany(p => p.ProductosOrden)
                .HasForeignKey(po => po.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
