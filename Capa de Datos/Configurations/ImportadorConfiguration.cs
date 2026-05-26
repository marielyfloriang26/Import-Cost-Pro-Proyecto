using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class ImportadorConfiguration : IEntityTypeConfiguration<Importador>
    {
        public void Configure(EntityTypeBuilder<Importador> builder)
        {
            builder.ToTable("Importadores");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.NombreRazonSocial)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(i => i.RncIdentificacion)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(i => i.RncIdentificacion)
                .IsUnique();

            builder.Property(i => i.Telefono)
                .HasMaxLength(20);

            builder.Property(i => i.Correo)
                .HasMaxLength(100);

            builder.Property(i => i.Direccion)
                .HasMaxLength(250);

            builder.Property(i => i.Estado)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(i => i.Pais)
                .WithMany(p => p.Importadores)
                .HasForeignKey(i => i.PaisId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
