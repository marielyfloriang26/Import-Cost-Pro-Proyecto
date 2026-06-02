using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Models.ViewModels;
using Capa_de_Negocio.Services.Interfaces;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrdenImportacionService _ordenService;
        private readonly IProductoService _productoService;
        private readonly IProveedorService _proveedorService;
        private readonly IImportadorService _importadorService;

        public HomeController(
            ILogger<HomeController> logger,
            IOrdenImportacionService ordenService,
            IProductoService productoService,
            IProveedorService proveedorService,
            IImportadorService importadorService)
        {
            _logger = logger;
            _ordenService = ordenService;
            _productoService = productoService;
            _proveedorService = proveedorService;
            _importadorService = importadorService;
        }

        public async Task<IActionResult> Index()
        {
            var ordenes = await _ordenService.ObtenerTodasAsync();
            var productos = await _productoService.ObtenerTodosAsync();
            var proveedores = await _proveedorService.ObtenerTodosAsync();
            var importadores = await _importadorService.ObtenerTodosAsync();

            var model = new DashboardViewModel
            {
                TotalOrdenes = ordenes.Count(),
                OrdenesAbiertas = ordenes.Count(o => o.EstadoOrden == Capa_de_Datos.Enums.EstadoOrden.Abierta),
                OrdenesCalculadas = ordenes.Count(o => o.EstadoOrden == Capa_de_Datos.Enums.EstadoOrden.Calculada),
                OrdenesCerradas = ordenes.Count(o => o.EstadoOrden == Capa_de_Datos.Enums.EstadoOrden.Cerrada),
                TotalFobAcumulado = ordenes.Sum(o => o.FobTotal),
                TotalProductos = productos.Count(),
                TotalProveedores = proveedores.Count(),
                TotalImportadores = importadores.Count(),
                Recientes = ordenes.OrderByDescending(o => o.FechaOrden)
                                   .Take(5)
                                   .Select(o => new OrdenImportacionIndexViewModel
                                   {
                                       Id = o.Id,
                                       NumeroOrden = o.NumeroOrden,
                                       ImportadorNombre = o.ImportadorNombre,
                                       ProveedorNombre = o.ProveedorNombre,
                                       FechaOrden = o.FechaOrden,
                                       EstadoOrden = o.EstadoOrden,
                                       FobTotal = o.FobTotal
                                   }).ToList()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
