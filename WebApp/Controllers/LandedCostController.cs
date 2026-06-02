using Capa_de_Datos.Enums;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class LandedCostController : Controller
    {
        private readonly ILandedCostService _landedCostService;
        private readonly IOrdenImportacionService _ordenService;

        public LandedCostController(ILandedCostService landedCostService, IOrdenImportacionService ordenService)
        {
            _landedCostService = landedCostService;
            _ordenService = ordenService;
        }

        // GET: LandedCost
        public async Task<IActionResult> Index()
        {
            var ordenesAbiertas = (await _ordenService.ObtenerAbiertasAsync()).OrderByDescending(o => o.FechaOrden).ToList();
            
            var viewModel = new LandedCostIndexViewModel
            {
                OrdenesAbiertas = new SelectList(ordenesAbiertas, "Id", "NumeroOrden"),
                SelectedOrdenId = ordenesAbiertas.FirstOrDefault()?.Id
            };

            return View(viewModel);
        }

        // POST: LandedCost/Calcular
        [HttpPost]
        public async Task<IActionResult> Calcular(LandedCostIndexViewModel model)
        {
            if (model.SelectedOrdenId == null)
            {
                ModelState.AddModelError("SelectedOrdenId", "Debe seleccionar una orden.");
                var ordenesAbiertas = await _ordenService.ObtenerAbiertasAsync();
                model.OrdenesAbiertas = new SelectList(ordenesAbiertas, "Id", "NumeroOrden");
                return View("Index", model);
            }

            try
            {
                var calculo = await _landedCostService.CalcularAsync(model.SelectedOrdenId.Value);
                var viewModel = new LandedCostCalculoViewModel
                {
                    Calculo = calculo,
                    EsCalculoOficial = false
                };
                return View("Resultado", viewModel);
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error inesperado durante el cálculo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: LandedCost/GuardarOficial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarOficial(int ordenId)
        {
            try
            {
                var calculo = await _landedCostService.GuardarCalculoOficialAsync(ordenId);
                TempData["Success"] = "El cálculo oficial ha sido guardado exitosamente. La orden ha sido marcada como CALCULADA.";
                
                var viewModel = new LandedCostCalculoViewModel
                {
                    Calculo = calculo,
                    EsCalculoOficial = true
                };
                return View("Resultado", viewModel);
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al guardar el cálculo oficial: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LandedCost/VerOficial/5
        public async Task<IActionResult> VerOficial(int ordenId)
        {
            var calculo = await _landedCostService.ObtenerPorOrdenIdAsync(ordenId);
            if (calculo == null)
            {
                TempData["Error"] = "No se encontró un cálculo oficial para esta orden.";
                return RedirectToAction("Details", "OrdenImportacion", new { id = ordenId });
            }

            var viewModel = new LandedCostCalculoViewModel
            {
                Calculo = calculo,
                EsCalculoOficial = true
            };
            return View("Resultado", viewModel);
        }
    }
}
