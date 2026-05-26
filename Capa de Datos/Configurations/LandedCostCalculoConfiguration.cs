using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class LandedCostCalculoConfiguration : IEntityTypeConfiguration<LandedCostCalculo>
    {
        public void Configure(EntityTypeBuilder<LandedCostCalculo> builder)
        {
            builder.ToTable("LandedCostCalculos");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.FechaCalculo)
                .IsRequired();

            builder.Property(l => l.TasaCambioUsada)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(l => l.PorcentajeItbisGeneral)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(l => l.PorcentajeTasaAduanal)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(l => l.FobTotalOriginal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.FobTotalLocal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.FleteTotalLocal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.SeguroTotalLocal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.CifTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.TotalArancel)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.TotalImpuestoSelectivo)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.TotalTasaServicio)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.TotalItbis)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.TotalGastosLocales)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.CostoTotalImportacion)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(l => l.CantidadTotalImportada)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(l => l.Orden)
                .WithOne(o => o.LandedCostCalculo)
                .HasForeignKey<LandedCostCalculo>(l => l.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.MonedaLocalUsada)
                .WithMany(m => m.LandedCostCalculos)
                .HasForeignKey(l => l.MonedaLocalUsadaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
