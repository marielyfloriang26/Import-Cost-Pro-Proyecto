using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedores");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Correo)
                .HasMaxLength(100);

            builder.Property(p => p.Telefono)
                .HasMaxLength(20);

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(p => p.Pais)
                .WithMany(pa => pa.Proveedores)
                .HasForeignKey(p => p.PaisId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.MonedaPrincipal)
                .WithMany(m => m.Proveedores)
                .HasForeignKey(p => p.MonedaPrincipalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
