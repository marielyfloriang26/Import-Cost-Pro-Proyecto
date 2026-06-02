using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.ViewModels; 
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;
    public class ConfiguracionImpuestosController : Controller
    {
        private readonly IConfiguracionImpuestoService _impuestoService;

        public ConfiguracionImpuestosController(IConfiguracionImpuestoService impuestoService)
        {
            _impuestoService = impuestoService;
        }

        // GET: ConfiguracionImpuestos
        public async Task<IActionResult> Index()
        {
            var dto = await _impuestoService.ObtenerConfiguracionActualAsync();
            
            if (dto.Id == 0)
        {
            // Retorna un modelo nuevo de paquete 
            // Al ser nuevo, las cajas de texto naceran completamente limpias y en blanco
            return View(new ConfiguracionImpuestoViewModel
            {
                Id = 0,
                PorcentajeItbis = null,
                PorcentajeTasaAduanal = null
            });
        }
            // si ya existe en la bd, retorna los datos reales para editarlos 
            // Mapeamos el DTO al ViewModel para la Vista
            var model = new ConfiguracionImpuestoViewModel
            {
                Id = dto.Id,
                PorcentajeItbis = null,
                PorcentajeTasaAduanal = null
            };

            return View(model);
        }

        // POST: ConfiguracionImpuestos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(ConfiguracionImpuestoViewModel model)
        {
            // Aquí se ejecutan las validaciones del ViewModel
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Mapeamos el ViewModel validado de vuelta al DTO para enviarlo al servicio
                var dto = new ConfiguracionImpuestoDto
                {
                    Id = model.Id,
                    PorcentajeItbis = model.PorcentajeItbis!.Value,
                    PorcentajeTasaAduanal = model.PorcentajeTasaAduanal!.Value
                };

                await _impuestoService.GuardarConfiguracionAsync(dto);
                TempData["SuccessMessage"] = "Configuración de impuestos guardada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al guardar: {ex.Message}");
                return View("Index", model);
            }
        }
    }
