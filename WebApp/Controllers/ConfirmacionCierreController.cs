using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class ConfirmacionCierreController : Controller
    {
        private readonly IConfirmacionCierreService _confirmacionCierreService;
        private readonly IOrdenImportacionService _ordenService;

        public ConfirmacionCierreController(
            IConfirmacionCierreService confirmacionCierreService,
            IOrdenImportacionService ordenService)
        {
            _confirmacionCierreService = confirmacionCierreService;
            _ordenService = ordenService;
        }

        // GET: ConfirmacionCierre/Confirmar/5
        public async Task<IActionResult> Confirmar(int id)
        {
            var orden = await _ordenService.ObtenerPorIdAsync(id);
            if (orden == null) return NotFound();

            return View(orden);
        }

        // POST: ConfirmacionCierre/Cerrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cerrar(int id)
        {
            try
            {
                await _confirmacionCierreService.CerrarOrdenAsync(id);
                TempData["Success"] = "La orden ha sido cerrada exitosamente y ahora es oficial e inmutable.";
                return RedirectToAction("VerOficial", "LandedCost", new { ordenId = id });
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Confirmar), new { id = id });
            }
        }
    }
}
