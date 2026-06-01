using Capa_de_Datos.Enums;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Capa_de_Negocio.Services.Interfaces;

namespace Capa_de_Web.Controllers;
    public class GastosImportacionController : Controller
    {
        private readonly IGastoImportacionService _gastoService;
        private readonly IOrdenImportacionService _ordenService; 
        private readonly IMonedaService _monedaService;

        public GastosImportacionController(
            IGastoImportacionService gastoService,
            IOrdenImportacionService ordenService,
            IMonedaService monedaService)
        {
            _gastoService = gastoService;
            _ordenService = ordenService;
            _monedaService = monedaService;
        }

        // GET: GastosImportacion / GastosImportacion?ordenId=5
        public async Task<IActionResult> Index(int? ordenId)
        {
            // Si el usuario no ha seleccionado una orden, carga la pantalla para que elija una
            if (ordenId == null || ordenId <= 0)
            {
                // Pasa las ordenes que estan Abiertas para que pueda seleccionar una
                var ordenesTodas = await _ordenService.ObtenerTodasAsync(); 
                var ordenesAbiertas = ordenesTodas.Where(o => o.EstadoOrden == EstadoOrden.Abierta);
                
                ViewBag.Ordenes = new SelectList(ordenesAbiertas, "Id", "NumeroOrden");
                return View("SeleccionarOrden");
            }

            // Si ya tenemos una orden seleccionada, listamos sus gastos
            var orden = await _ordenService.ObtenerPorIdAsync(ordenId.Value);
            if (orden == null) return NotFound();

            var gastosDto = await _gastoService.ObtenerPorOrdenIdAsync(ordenId.Value);
            
            // Mapea los DTOs a ViewModels para la vista
            var listaGastos = gastosDto.Select(g => new GastoImportacionViewModel
            {
                Id = g.Id,
                OrdenId = g.OrdenId,
                OrdenCodigo = g.OrdenCodigo,
                TipoGasto = g.TipoGasto,
                Monto = g.Monto,
                MonedaId = g.MonedaId,
                MonedaNombre = g.MonedaNombre,
                MetodoDistribucion = g.MetodoDistribucion,
                FechaGasto = g.FechaGasto
            }).ToList();

            ViewBag.OrdenId = orden.Id;
            ViewBag.NumeroOrden = orden.NumeroOrden;
            ViewBag.EstadoOrden = orden.EstadoOrden.ToString(); // Para validar botones en la vista

            return View(listaGastos);
        }

        // GET: GastosImportacion/Registrar?ordenId=5
        public async Task<IActionResult> Registrar(int ordenId)
        {
            var orden = await _ordenService.ObtenerPorIdAsync(ordenId);
            if (orden == null) return NotFound();

            if (orden.EstadoOrden != EstadoOrden.Abierta)
            {
                TempData["ErrorMessage"] = "No se pueden registrar gastos en una orden calculada, cerrada o cancelada.";
                return RedirectToAction(nameof(Index), new { ordenId = ordenId });
            }

            var model = new GastoImportacionViewModel
            {
                OrdenId = orden.Id,
                OrdenCodigo = orden.NumeroOrden,
                FechaGasto = DateTime.Now
            };

            await CargarDropdownsAsync(model.MonedaId);
            return View(model);
        }

        // POST: GastosImportacion/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(GastoImportacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdownsAsync(model.MonedaId);
                return View(model);
            }

            try
            {
                var dto = new GastoImportacionDto
                {
                    OrdenId = model.OrdenId,
                    TipoGasto = model.TipoGasto,
                    Monto = model.Monto,
                    MonedaId = model.MonedaId,
                    MetodoDistribucion = model.MetodoDistribucion,
                    FechaGasto = model.FechaGasto
                };

                await _gastoService.RegistrarAsync(dto);
                TempData["SuccessMessage"] = "Gasto de importación registrado correctamente.";
                return RedirectToAction(nameof(Index), new { ordenId = model.OrdenId });
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarDropdownsAsync(model.MonedaId);
                return View(model);
            }
        }

        // GET: GastosImportacion/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var gastoDto = await _gastoService.ObtenerPorIdAsync(id);
            if (gastoDto == null) return NotFound();

            var model = new GastoImportacionViewModel
            {
                Id = gastoDto.Id,
                OrdenId = gastoDto.OrdenId,
                OrdenCodigo = gastoDto.OrdenCodigo,
                TipoGasto = gastoDto.TipoGasto,
                Monto = gastoDto.Monto,
                MonedaId = gastoDto.MonedaId,
                MetodoDistribucion = gastoDto.MetodoDistribucion,
                FechaGasto = gastoDto.FechaGasto
            };

            await CargarDropdownsAsync(model.MonedaId, esEdicion: true);
            return View(model);
        }

        // POST: GastosImportacion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, GastoImportacionViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarDropdownsAsync(model.MonedaId, esEdicion: true);
                return View(model);
            }

            try
            {
                var dto = new GastoImportacionDto
                {
                    Id = model.Id,
                    OrdenId = model.OrdenId,
                    TipoGasto = model.TipoGasto,
                    Monto = model.Monto,
                    MonedaId = model.MonedaId,
                    MetodoDistribucion = model.MetodoDistribucion,
                    FechaGasto = model.FechaGasto
                };

                await _gastoService.EditarAsync(dto);
                TempData["SuccessMessage"] = "Gasto de importación modificado correctamente.";
                return RedirectToAction(nameof(Index), new { ordenId = model.OrdenId });
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarDropdownsAsync(model.MonedaId, esEdicion: true);
                return View(model);
            }
        }

        // GET: GastosImportacion/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var gastoDto = await _gastoService.ObtenerPorIdAsync(id);
            if (gastoDto == null) return NotFound();

            var model = new GastoImportacionViewModel
            {
                Id = gastoDto.Id,
                OrdenId = gastoDto.OrdenId,
                OrdenCodigo = gastoDto.OrdenCodigo,
                TipoGasto = gastoDto.TipoGasto,
                Monto = gastoDto.Monto,
                MonedaNombre = gastoDto.MonedaNombre,
                MetodoDistribucion = gastoDto.MetodoDistribucion,
                FechaGasto = gastoDto.FechaGasto
            };

            return View(model);
        }

        // POST: GastosImportacion/EliminarConfirmado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var gastoDto = await _gastoService.ObtenerPorIdAsync(id);
            if (gastoDto == null) return NotFound();

            try
            {
                await _gastoService.EliminarAsync(id);
                TempData["SuccessMessage"] = "Gasto de importación eliminado correctamente.";
                return RedirectToAction(nameof(Index), new { ordenId = gastoDto.OrdenId });
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index), new { ordenId = gastoDto.OrdenId });
            }
        }

        // Metodo de apoyo para inyectar listas desplegables a las vistas
        private async Task CargarDropdownsAsync(int monedaSeleccionadaId, bool esEdicion = false)
        {
            // Monedas, Si es creacion solo activas. Si es edición, todas para preservar el historico
            var monedas = esEdicion ? await _monedaService.ObtenerTodasAsync() : await _monedaService.ObtenerActivasAsync();
            ViewBag.Monedas = new SelectList(monedas, "Id", "Nombre", monedaSeleccionadaId);

            // Enum TipoGasto traducido a select list limpia
            var tiposGastos = Enum.GetValues(typeof(TipoGasto)).Cast<TipoGasto>().Select(t => new { Id = (int)t, Nombre = t.ToString() });
            ViewBag.TiposGastos = new SelectList(tiposGastos, "Id", "Nombre");

            // Enum MetodoDistribucion traducido a select list limpia
            var metodos = Enum.GetValues(typeof(MetodoDistribucion)).Cast<MetodoDistribucion>().Select(m => new { Id = (int)m, Nombre = m.ToString() });
            ViewBag.MetodosDistribucion = new SelectList(metodos, "Id", "Nombre");
        }
    }
