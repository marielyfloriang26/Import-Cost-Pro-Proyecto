using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class OrdenImportacionConfiguration : IEntityTypeConfiguration<OrdenImportacion>
    {
        public void Configure(EntityTypeBuilder<OrdenImportacion> builder)
        {
            builder.ToTable("OrdenesImportacion");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.NumeroOrden)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(o => o.NumeroOrden)
                .IsUnique();

            builder.Property(o => o.FechaOrden)
                .IsRequired();

            builder.Property(o => o.MedioTransporte)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.EstadoOrden)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Capa_de_Datos.Enums.EstadoOrden.Abierta);

            builder.HasOne(o => o.Importador)
                .WithMany(i => i.OrdenesImportacion)
                .HasForeignKey(o => o.ImportadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Proveedor)
                .WithMany(p => p.OrdenesImportacion)
                .HasForeignKey(o => o.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.PaisOrigen)
                .WithMany(p => p.OrdenesImportacion)
                .HasForeignKey(o => o.PaisOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Moneda)
                .WithMany(m => m.OrdenesImportacion)
                .HasForeignKey(o => o.MonedaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
