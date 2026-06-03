using System;
using System.Threading.Tasks;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class DetalleOrdenController : Controller
    {
        private readonly IProductoOrdenService _productoOrdenService;

        public DetalleOrdenController(IProductoOrdenService productoOrdenService)
        {
            _productoOrdenService = productoOrdenService;
        }

        // GET: DetalleOrden/Index/id
        public IActionResult Index(int id)
        {
            return RedirectToAction("Details", "OrdenImportacion", new { id = id });
        }

        // GET: DetalleOrden/AgregarProducto?ordenId=id
        public async Task<IActionResult> AgregarProducto(int ordenId)
        {
            var productosActivos = await _productoOrdenService.ObtenerProductosActivosAsync();
            ViewBag.Productos = new SelectList(productosActivos, "ProductoId", "NombreProducto");
            return View(new ProductoOrdenViewModel { OrdenImportacionId = ordenId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(ProductoOrdenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var productosActivos = await _productoOrdenService.ObtenerProductosActivosAsync();
                ViewBag.Productos = new SelectList(productosActivos, "ProductoId", "NombreProducto", model.ProductoId);
                return View(model);
            }

            try
            {
                var dto = new ProductoOrdenDto
                {
                    OrdenImportacionId = model.OrdenImportacionId,
                    ProductoId = model.ProductoId,
                    Cantidad = model.Cantidad,
                    PrecioUnitarioFob = model.PrecioUnitarioFob,
                    MargenGananciaDeseado = model.MargenGananciaDeseado
                };

                await _productoOrdenService.AgregarProductoAOrdenAsync(dto);
                return RedirectToAction("Details", "OrdenImportacion", new { id = model.OrdenImportacionId });
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var productosActivos = await _productoOrdenService.ObtenerProductosActivosAsync();
                ViewBag.Productos = new SelectList(productosActivos, "ProductoId", "NombreProducto", model.ProductoId);
                return View(model);
            }
        }

        // GET: DetalleOrden/EditarProducto/id
        public async Task<IActionResult> EditarProducto(int id)
        {
            var dto = await _productoOrdenService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new ProductoOrdenViewModel
            {
                Id = dto.Id,
                OrdenImportacionId = dto.OrdenImportacionId,
                ProductoId = dto.ProductoId,
                NombreProducto = dto.NombreProducto,
                Cantidad = dto.Cantidad,
                PrecioUnitarioFob = dto.PrecioUnitarioFob,
                MargenGananciaDeseado = dto.MargenGananciaDeseado
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(int id, ProductoOrdenViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new ProductoOrdenDto
                {
                    Id = model.Id,
                    Cantidad = model.Cantidad,
                    PrecioUnitarioFob = model.PrecioUnitarioFob,
                    MargenGananciaDeseado = model.MargenGananciaDeseado
                };

                await _productoOrdenService.EditarProductoEnOrdenAsync(dto);
                return RedirectToAction("Details", "OrdenImportacion", new { id = model.OrdenImportacionId });
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: DetalleOrden/EliminarProducto/id
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var dto = await _productoOrdenService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new ProductoOrdenViewModel
            {
                Id = dto.Id,
                OrdenImportacionId = dto.OrdenImportacionId,
                NombreProducto = dto.NombreProducto,
                Cantidad = dto.Cantidad
            };
            return View(model);
        }

        [HttpPost, ActionName("EliminarProducto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id, int ordenId)
        {
            try
            {
                await _productoOrdenService.EliminarProductoDeOrdenAsync(id);
                return RedirectToAction("Details", "OrdenImportacion", new { id = ordenId });
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(EliminarProducto), new { id });
            }
        }
    }
}
