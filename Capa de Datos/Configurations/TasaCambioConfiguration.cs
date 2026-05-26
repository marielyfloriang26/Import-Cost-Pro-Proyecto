using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class TasaCambioConfiguration : IEntityTypeConfiguration<TasaCambio>
    {
        public void Configure(EntityTypeBuilder<TasaCambio> builder)
        {
            builder.ToTable("TasasCambio");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.ValorTasa)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(t => t.FechaVigencia)
                .IsRequired();

            builder.Property(t => t.Estado)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(t => t.MonedaOrigen)
                .WithMany(m => m.TasasOrigen)
                .HasForeignKey(t => t.MonedaOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.MonedaDestino)
                .WithMany(m => m.TasasDestino)
                .HasForeignKey(t => t.MonedaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
