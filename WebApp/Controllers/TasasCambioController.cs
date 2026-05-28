using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using Capa_de_Datos.Repositories.Interfaces;
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
    private readonly IMonedaRepository _monedaRepository;

    public TasasCambioController(ITasasDeCambioService service, IMonedaRepository monedaRepository)
    {
        _service = service;
        _monedaRepository = monedaRepository;
    }

    public async Task<IActionResult> Index()
    {
        var tasasDto = await _service.GetTasas();
        var monedas = await _monedaRepository.GetAllAsync();

        var viewModels = tasasDto.Select(t => new TasaCambioIndexViewModel
        {
            IdTasaCambio = t.IdTasaCambio,
            MonedaOrigenNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaOrigenId)?.Nombre ?? "N/A",
            MonedaDestinoNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaDestinoId)?.Nombre ?? "N/A",
            ValorTasa = t.ValorTasa,
            FechaVigencia = t.FechaVigencia,
            Estado = t.Estado
        });

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var t = await _service.GetTasaById(id);
            var monedas = await _monedaRepository.GetAllAsync();

            var viewModel = new TasaCambioIndexViewModel
            {
                IdTasaCambio = t.IdTasaCambio,
                MonedaOrigenNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaOrigenId)?.Nombre ?? "N/A",
                MonedaDestinoNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaDestinoId)?.Nombre ?? "N/A",
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

    public async Task<IActionResult> Create()
    {
        await CargarMonedasEnViewBag();
        return View(new TasaCambioFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TasaCambioFormViewModel viewModel)
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

    public async Task<IActionResult> Edit(int id)
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TasaCambioFormViewModel viewModel)
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

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var t = await _service.GetTasaById(id);
            var monedas = await _monedaRepository.GetAllAsync();

            var viewModel = new TasaCambioIndexViewModel
            {
                IdTasaCambio = t.IdTasaCambio,
                MonedaOrigenNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaOrigenId)?.Nombre ?? "N/A",
                MonedaDestinoNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaDestinoId)?.Nombre ?? "N/A",
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

    [HttpPost, ActionName("Delete")]
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
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (TasaCambioNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task CargarMonedasEnViewBag()
    {
        var monedas = await _monedaRepository.GetAllAsync();
        ViewBag.Monedas = new SelectList(monedas.Where(m => m.Estado), "Id", "Nombre");
    }
}