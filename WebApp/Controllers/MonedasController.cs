using System;
using System.Linq;
using System.Threading.Tasks;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class MonedasController : Controller
    {
        private readonly IMonedaService _monedaService;

        public MonedasController(IMonedaService monedaService)
        {
            _monedaService = monedaService;
        }

        // GET: Monedas
        public async Task<IActionResult> Index()
        {
            var dtos = await _monedaService.ObtenerTodasAsync();
            var model = dtos.Select(m => new MonedaViewModel
            {
                Id = m.Id,
                Nombre = m.Nombre,
                CodigoIso = m.CodigoIso,
                Simbolo = m.Simbolo,
                EsMonedaLocal = m.EsMonedaLocal,
                Estado = m.Estado
            });
            return View(model);
        }

        // GET: Monedas/Crear
        public IActionResult Crear()
        {
            return View(new MonedaViewModel());
        }

        // POST: Monedas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MonedaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new MonedaDto
                {
                    Nombre = model.Nombre,
                    CodigoIso = model.CodigoIso,
                    Simbolo = model.Simbolo,
                    EsMonedaLocal = model.EsMonedaLocal,
                    Estado = model.Estado
                };

                await _monedaService.CrearAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                // Agrega el error de negocio al resumen para que se pinte en la vista
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: Monedas/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _monedaService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new MonedaViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                CodigoIso = dto.CodigoIso,
                Simbolo = dto.Simbolo,
                EsMonedaLocal = dto.EsMonedaLocal,
                Estado = dto.Estado
            };
            return View(model);
        }

        // POST: Monedas/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, MonedaViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new MonedaDto
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    CodigoIso = model.CodigoIso,
                    Simbolo = model.Simbolo,
                    EsMonedaLocal = model.EsMonedaLocal,
                    Estado = model.Estado
                };

                await _monedaService.EditarAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: Monedas/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var dto = await _monedaService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new MonedaViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                CodigoIso = dto.CodigoIso,
                Simbolo = dto.Simbolo,
                EsMonedaLocal = dto.EsMonedaLocal,
                Estado = dto.Estado
            };
            return View(model);
        }

        // POST: Monedas/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _monedaService.EliminarAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                // Usamos TempData para pasar el mensaje de error de negocio a la vista GET de confirmación
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Eliminar), new { id });
            }
        }
    }
}