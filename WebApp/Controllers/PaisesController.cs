using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PaisesController : Controller
    {
        private readonly IPaisService _paisService;

        public PaisesController(IPaisService paisService)
        {
            _paisService = paisService;
        }

        // GET: Paises
        public async Task<IActionResult> Index()
        {
            var dtos = await _paisService.ObtenerTodosAsync();
            var model = dtos.Select(p => new PaisViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoIso = p.CodigoIso,
                Estado = p.Estado
            });
            return View(model);
        }

        // GET: Paises/Crear
        public IActionResult Crear()
        {
            return View(new PaisViewModel());
        }

        // POST: Paises/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PaisViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new PaisDto
                {
                    Nombre = model.Nombre,
                    CodigoIso = model.CodigoIso,
                    Estado = model.Estado
                };

                await _paisService.CrearAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: Paises/Editar
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _paisService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new PaisViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                CodigoIso = dto.CodigoIso,
                Estado = dto.Estado
            };
            return View(model);
        }

        // POST: Paises/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, PaisViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var dto = new PaisDto
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    CodigoIso = model.CodigoIso,
                    Estado = model.Estado
                };

                await _paisService.EditarAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: Paises/Eliminar
        public async Task<IActionResult> Eliminar(int id)
        {
            var dto = await _paisService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new PaisViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                CodigoIso = dto.CodigoIso,
                Estado = dto.Estado
            };
            return View(model);
        }

        // POST: Paises/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _paisService.EliminarAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Eliminar), new { id });
            }
        }
    }
}