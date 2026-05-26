using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Configurations
{
    public class ConfiguracionImpuestoConfiguration : IEntityTypeConfiguration<ConfiguracionImpuesto>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionImpuesto> builder)
        {
            builder.ToTable("ConfiguracionImpuestos");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.PorcentajeItbis)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(c => c.PorcentajeTasaAduanal)
                .IsRequired()
                .HasPrecision(5, 2);
        }
    }
}
