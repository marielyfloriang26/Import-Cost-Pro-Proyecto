using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class LandedCostDetalleConfiguration : IEntityTypeConfiguration<LandedCostDetalle>
    {
        public void Configure(EntityTypeBuilder<LandedCostDetalle> builder)
        {
            builder.ToTable("LandedCostDetalles");

            builder.HasKey(ld => ld.Id);

            builder.Property(ld => ld.Cantidad)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.FobOriginal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.FobLocal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.FleteAsignado)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.SeguroAsignado)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.Cif)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.PorcentajeArancelUsado)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(ld => ld.Arancel)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.PorcentajeSelectivoUsado)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(ld => ld.ImpuestoSelectivo)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.TasaServicioAduanal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.Itbis)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.GastosLocalesAsignados)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.CostoTotalImportado)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.CostoUnitarioImportado)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ld => ld.MargenDeseado)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(ld => ld.PrecioVentaSugerido)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(ld => ld.Calculo)
                .WithMany(c => c.Detalles)
                .HasForeignKey(ld => ld.CalculoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ld => ld.Producto)
                .WithMany(p => p.LandedCostDetalles)
                .HasForeignKey(ld => ld.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
