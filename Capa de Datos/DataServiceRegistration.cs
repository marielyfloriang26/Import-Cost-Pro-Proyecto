using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Capa_de_Datos.Context;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Datos.Repositories.Implementations;

namespace Capa_de_Datos
{
    public static class DataServiceRegistration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ImportCostContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registro de Repositorios
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IMonedaRepository, MonedaRepository>();
            services.AddScoped<IImportadorRepository, ImportadorRepository>();
            services.AddScoped<IProveedorRepository, ProveedorRepository>();
            services.AddScoped<ICategoriaArancelariaRepository, CategoriaArancelariaRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<ITasaCambioRepository, TasaCambioRepository>();
            services.AddScoped<IOrdenImportacionRepository, OrdenImportacionRepository>();
            services.AddScoped<IProductoOrdenRepository, ProductoOrdenRepository>();
            services.AddScoped<IGastoImportacionRepository, GastoImportacionRepository>();
            services.AddScoped<IConfiguracionImpuestoRepository, ConfiguracionImpuestoRepository>();
            services.AddScoped<ILandedCostCalculoRepository, LandedCostCalculoRepository>();
            services.AddScoped<ILandedCostDetalleRepository, LandedCostDetalleRepository>();

            return services;
        }
    }
}
