using Capa_de_Datos;
using Capa_de_Datos.Repositories.Implementations;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.Implementations;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.Services;
using Capa_de_Negocio.Services.Implementations;
using Capa_de_Negocio.Servicios;
using Capa_Negocio.Implementations;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IMonedaRepository, MonedaRepository>();
            builder.Services.AddScoped<IMonedaService, MonedaService>();

            // Registrar Servicios de Negocio
            builder.Services.AddScoped<Capa_de_Negocio.Services.Interfaces.ITasasDeCambioService, Capa_de_Negocio.Services.Implementations.TasasDeCambioService>();
            builder.Services.AddScoped<Capa_de_Negocio.Services.Interfaces.IOrdenImportacionService, Capa_de_Negocio.Services.Implementations.OrdenImportacionService>();

            // Configurar Capa de Datos
            builder.Services.AddDataServices(builder.Configuration);
            builder.Services.AddScoped<IPaisRepository, PaisRepository>();
            builder.Services.AddScoped<IPaisService, PaisService>();
            builder.Services.AddScoped<IImportadorRepository, ImportadorRepository>();
            builder.Services.AddScoped<IImportadorService, ImportadorService>();
            
            builder.Services.AddScoped<IGastoImportacionRepository, GastoImportacionRepository>();
            builder.Services.AddScoped<IGastoImportacionService, GastoImportacionService>();
            builder.Services.AddScoped<IProductoOrdenRepository, ProductoOrdenRepository>();
            builder.Services.AddScoped<IProductoOrdenService, ProductoOrdenService>();

            builder.Services.AddScoped<IProveedorService, ProveedorService>();
            builder.Services.AddScoped<ICategoriaArancelariaService, CategoriaArancelariaService>();
            builder.Services.AddScoped<IProductoService, ProductoService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
