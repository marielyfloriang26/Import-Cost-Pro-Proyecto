using Microsoft.EntityFrameworkCore;
using Capa_de_Datos.Entities;
using System.Reflection;

namespace Capa_de_Datos.Context
{
    public class ImportCostContext : DbContext
    {
        public ImportCostContext(DbContextOptions<ImportCostContext> options) : base(options)
        {
        }

        public DbSet<Pais> Paises { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Importador> Importadores { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<CategoriaArancelaria> CategoriasArancelarias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TasaCambio> TasasCambio { get; set; }
        public DbSet<OrdenImportacion> OrdenesImportacion { get; set; }
        public DbSet<ProductoOrden> ProductosOrden { get; set; }
        public DbSet<GastoImportacion> GastosImportacion { get; set; }
        public DbSet<ConfiguracionImpuesto> ConfiguracionImpuestos { get; set; }
        public DbSet<LandedCostCalculo> LandedCostCalculos { get; set; }
        public DbSet<LandedCostDetalle> LandedCostDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Aplica todas las configuraciones que implementan IEntityTypeConfiguration
            // en el mismo ensamblado donde está este DbContext
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
