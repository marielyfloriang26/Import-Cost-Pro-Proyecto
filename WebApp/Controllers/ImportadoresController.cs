using System;
using System.Linq;
using System.Threading.Tasks;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class ImportadoresController : Controller
    {
        private readonly IImportadorService _importadorService;
        private readonly IPaisService _paisService;

        public ImportadoresController(IImportadorService importadorService, IPaisService paisService)
        {
            _importadorService = importadorService;
            _paisService = paisService;
        }

        // GET: Importadores
        public async Task<IActionResult> Index()
        {
            var dtos = await _importadorService.ObtenerTodosAsync();
            var model = dtos.Select(i => new ImportadorViewModel
            {
                Id = i.Id,
                NombreRazonSocial = i.NombreRazonSocial,
                RncIdentificacion = i.RncIdentificacion,
                NombrePais = i.NombrePais,
                Telefono = i.Telefono,
                Correo = i.Correo,
                Estado = i.Estado
            });
            return View(model);
        }

        // GET: Importadores/Crear
        public async Task<IActionResult> Crear()
        {
            // REGLA DEL PDF: Al crear solo deben mostrarse países activos
            var paisesActivos = await _paisService.ObtenerActivosAsync();
            ViewBag.Paises = new SelectList(paisesActivos, "Id", "Nombre");
            
            return View(new ImportadorViewModel { Estado = true });
        }

        // POST: Importadores/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ImportadorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var paisesActivos = await _paisService.ObtenerActivosAsync();
                ViewBag.Paises = new SelectList(paisesActivos, "Id", "Nombre", model.PaisId);
                return View(model);
            }

            try
            {
                var dto = new ImportadorDto
                {
                    NombreRazonSocial = model.NombreRazonSocial,
                    RncIdentificacion = model.RncIdentificacion,
                    PaisId = model.PaisId,
                    Telefono = model.Telefono,
                    Correo = model.Correo,
                    Direccion = model.Direccion,
                    Estado = model.Estado
                };

                await _importadorService.CrearAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var paisesActivos = await _paisService.ObtenerActivosAsync();
                ViewBag.Paises = new SelectList(paisesActivos, "Id", "Nombre", model.PaisId);
                return View(model);
            }
        }

        // GET: Importadores/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _importadorService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            // REGLA DEL PDF: En la edición se muestran todos los países (incluso inactivos) 
            // para mantener la visualización histórica del registro amarrado.
            var todosLosPaises = await _paisService.ObtenerTodosAsync();
            ViewBag.Paises = new SelectList(todosLosPaises, "Id", "Nombre", dto.PaisId);

            var model = new ImportadorViewModel
            {
                Id = dto.Id,
                NombreRazonSocial = dto.NombreRazonSocial,
                RncIdentificacion = dto.RncIdentificacion,
                PaisId = dto.PaisId,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                Direccion = dto.Direccion,
                Estado = dto.Estado
            };
            return View(model);
        }

        // POST: Importadores/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ImportadorViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var todosLosPaises = await _paisService.ObtenerTodosAsync();
                ViewBag.Paises = new SelectList(todosLosPaises, "Id", "Nombre", model.PaisId);
                return View(model);
            }

            try
            {
                var dto = new ImportadorDto
                {
                    Id = model.Id,
                    NombreRazonSocial = model.NombreRazonSocial,
                    RncIdentificacion = model.RncIdentificacion,
                    PaisId = model.PaisId,
                    Telefono = model.Telefono,
                    Correo = model.Correo,
                    Direccion = model.Direccion,
                    Estado = model.Estado
                };

                await _importadorService.EditarAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var todosLosPaises = await _paisService.ObtenerTodosAsync();
                ViewBag.Paises = new SelectList(todosLosPaises, "Id", "Nombre", model.PaisId);
                return View(model);
            }
        }

        // GET: Importadores/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var dto = await _importadorService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var model = new ImportadorViewModel
            {
                Id = dto.Id,
                NombreRazonSocial = dto.NombreRazonSocial,
                RncIdentificacion = dto.RncIdentificacion
            };
            return View(model);
        }

        // POST: Importadores/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _importadorService.EliminarAsync(id);
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