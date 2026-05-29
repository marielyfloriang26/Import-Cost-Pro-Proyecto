using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Controllers;

public class TasasCambioController : Controller
{
    private readonly ITasasDeCambioService _service;
    private readonly IMonedaService _monedaService;

    public TasasCambioController(ITasasDeCambioService service, IMonedaService monedaService)
    {
        _service = service;
        _monedaService = monedaService;
    }

    // GET: TasasCambio
    public async Task<IActionResult> Index()
    {
        var tasasDto = await _service.GetTasas();

        var viewModels = tasasDto.Select(t => new TasaCambioIndexViewModel
        {
            IdTasaCambio = t.IdTasaCambio,
            MonedaOrigenNombre = t.MonedaOrigenNombre,
            MonedaDestinoNombre = t.MonedaDestinoNombre,
            ValorTasa = t.ValorTasa,
            FechaVigencia = t.FechaVigencia,
            Estado = t.Estado
        });

        return View(viewModels);
    }

    // GET: TasasCambio/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var t = await _service.GetTasaById(id);

            var viewModel = new TasaCambioIndexViewModel
            {
                IdTasaCambio = t.IdTasaCambio,
                MonedaOrigenNombre = t.MonedaOrigenNombre,
                MonedaDestinoNombre = t.MonedaDestinoNombre,
                ValorTasa = t.ValorTasa,
                FechaVigencia = t.FechaVigencia,
                Estado = t.Estado
            };

            return View(viewModel);
        }
        catch (TasaCambioNotFoundException)
        {
            return NotFound();
        }
    }

    // GET: TasasCambio/Crear
    public async Task<IActionResult> Crear()
    {
        await CargarMonedasEnViewBag();
        return View(new TasaCambioFormViewModel());
    }

    // POST: TasasCambio/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(TasaCambioFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await CargarMonedasEnViewBag();
            return View(viewModel);
        }

        try
        {
            var dto = new CrearTasaCambioDto
            {
                MonedaOrigenId = viewModel.MonedaOrigenId,
                MonedaDestinoId = viewModel.MonedaDestinoId,
                ValorTasa = viewModel.ValorTasa,
                FechaVigencia = viewModel.FechaVigencia,
                Estado = viewModel.Estado
            };

            await _service.CreateTasa(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is SameCurrencyNotAllowedException || 
                                   ex is InactiveCurrencyException || 
                                   ex is LocalCurrencyRequiredException || 
                                   ex is InvalidExchangeRateValueException || 
                                   ex is DuplicateExchangeRateException ||
                                   ex is CurrencyNotFoundException)
        {
            ModelState.AddModelError("", ex.Message);
            await CargarMonedasEnViewBag();
            return View(viewModel);
        }
    }

    // GET: TasasCambio/Editar/5
    public async Task<IActionResult> Editar(int id)
    {
        try
        {
            var t = await _service.GetTasaById(id);
            
            var viewModel = new TasaCambioFormViewModel
            {
                IdTasaCambio = t.IdTasaCambio,
                MonedaOrigenId = t.MonedaOrigenId,
                MonedaDestinoId = t.MonedaDestinoId,
                ValorTasa = t.ValorTasa,
                FechaVigencia = t.FechaVigencia,
                Estado = t.Estado
            };

            await CargarMonedasEnViewBag();
            return View(viewModel);
        }
        catch (TasaCambioNotFoundException)
        {
            return NotFound();
        }
    }

    // POST: TasasCambio/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, TasaCambioFormViewModel viewModel)
    {
        if (id != viewModel.IdTasaCambio) return BadRequest();

        if (!ModelState.IsValid)
        {
            await CargarMonedasEnViewBag();
            return View(viewModel);
        }

        try
        {
            var dto = new TasaCambioDto
            {
                IdTasaCambio = viewModel.IdTasaCambio,
                MonedaOrigenId = viewModel.MonedaOrigenId,
                MonedaDestinoId = viewModel.MonedaDestinoId,
                ValorTasa = viewModel.ValorTasa,
                FechaVigencia = viewModel.FechaVigencia,
                Estado = viewModel.Estado
            };

            await _service.UpdateTasa(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (TasaCambioLockedException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await CargarMonedasEnViewBag();
            return View(viewModel);
        }
        catch (Exception ex) when (ex is SameCurrencyNotAllowedException || 
                                   ex is InactiveCurrencyException || 
                                   ex is LocalCurrencyRequiredException || 
                                   ex is InvalidExchangeRateValueException || 
                                   ex is DuplicateExchangeRateException ||
                                   ex is CurrencyNotFoundException ||
                                   ex is TasaCambioNotFoundException)
        {
            ModelState.AddModelError("", ex.Message);
            await CargarMonedasEnViewBag();
            return View(viewModel);
        }
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var t = await _service.GetTasaById(id);

            var viewModel = new TasaCambioIndexViewModel
            {
                IdTasaCambio = t.IdTasaCambio,
                MonedaOrigenNombre = t.MonedaOrigenNombre,
                MonedaDestinoNombre = t.MonedaDestinoNombre,
                ValorTasa = t.ValorTasa,
                FechaVigencia = t.FechaVigencia,
                Estado = t.Estado
            };

            return View(viewModel);
        }
        catch (TasaCambioNotFoundException)
        {
            return NotFound();
        }
    }

    // POST: TasasCambio/Eliminar/5
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _service.DeleteTasa(id);
            return RedirectToAction(nameof(Index));
        }
        catch (TasaCambioLockedException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Eliminar), new { id });
        }
        catch (TasaCambioNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task CargarMonedasEnViewBag()
    {
        var monedas = await _monedaService.ObtenerActivasAsync();
        ViewBag.Monedas = new SelectList(monedas, "Id", "Nombre");
    }
}