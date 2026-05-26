using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class GastoImportacionConfiguration : IEntityTypeConfiguration<GastoImportacion>
    {
        public void Configure(EntityTypeBuilder<GastoImportacion> builder)
        {
            builder.ToTable("GastosImportacion");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.TipoGasto)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(g => g.Monto)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(g => g.MetodoDistribucion)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(g => g.FechaGasto)
                .IsRequired();

            builder.HasOne(g => g.Orden)
                .WithMany(o => o.GastosImportacion)
                .HasForeignKey(g => g.OrdenId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.Moneda)
                .WithMany(m => m.GastosImportacion)
                .HasForeignKey(g => g.MonedaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
