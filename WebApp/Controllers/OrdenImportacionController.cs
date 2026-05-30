using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capa_de_Datos.Enums;

namespace WebApp.Controllers;

public class OrdenImportacionController : Controller
{
    private readonly IOrdenImportacionService _service;
    private readonly IImportadorService _importadorService;
    private readonly IProveedorService _proveedorService;
    private readonly IPaisService _paisService;
    private readonly IMonedaService _monedaService;

    public OrdenImportacionController(
        IOrdenImportacionService service,
        IImportadorService importadorService,
        IProveedorService proveedorService,
        IPaisService paisService,
        IMonedaService monedaService)
    {
        _service = service;
        _importadorService = importadorService;
        _proveedorService = proveedorService;
        _paisService = paisService;
        _monedaService = monedaService;
    }

    // GET: OrdenImportacion
    public async Task<IActionResult> Index()
    {
        var ordenesDto = await _service.ObtenerTodasAsync();

        var viewModels = ordenesDto.Select(o => new OrdenImportacionIndexViewModel
        {
            Id = o.Id,
            NumeroOrden = o.NumeroOrden,
            ImportadorNombre = o.ImportadorNombre,
            ProveedorNombre = o.ProveedorNombre,
            PaisOrigenNombre = o.PaisOrigenNombre,
            MonedaNombre = o.MonedaNombre,
            FechaOrden = o.FechaOrden,
            MedioTransporte = o.MedioTransporte,
            EstadoOrden = o.EstadoOrden,
            FobTotal = o.FobTotal,
            TotalEstimadoLandedCost = o.TotalEstimadoLandedCost
        });

        return View(viewModels);
    }

    // GET: OrdenImportacion/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var o = await _service.ObtenerPorIdAsync(id);
        if (o == null) return NotFound();

        var viewModel = new OrdenImportacionIndexViewModel
        {
            Id = o.Id,
            NumeroOrden = o.NumeroOrden,
            ImportadorNombre = o.ImportadorNombre,
            ProveedorNombre = o.ProveedorNombre,
            PaisOrigenNombre = o.PaisOrigenNombre,
            MonedaNombre = o.MonedaNombre,
            FechaOrden = o.FechaOrden,
            MedioTransporte = o.MedioTransporte,
            EstadoOrden = o.EstadoOrden,
            FobTotal = o.FobTotal,
            TotalEstimadoLandedCost = o.TotalEstimadoLandedCost
        };

        return View(viewModel);
    }

    // GET: OrdenImportacion/Crear
    public async Task<IActionResult> Crear()
    {
        await CargarCatalogosEnViewBag();
        return View(new OrdenImportacionFormViewModel());
    }

        // POST: OrdenImportacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenImportacionFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosEnViewBag();
                return View(viewModel);
            }

            try
            {
                var dto = new CrearOrdenImportacionDto
                {
                    NumeroOrden = viewModel.NumeroOrden,
                    ImportadorId = viewModel.ImportadorId,
                    ProveedorId = viewModel.ProveedorId,
                    PaisOrigenId = viewModel.PaisOrigenId,
                    MonedaId = viewModel.MonedaId,
                    FechaOrden = viewModel.FechaOrden,
                    MedioTransporte = viewModel.MedioTransporte
                };

                var nuevaOrden = await _service.CrearAsync(dto);
                // Redirigir al detalle según requisito pág. 77
                return RedirectToAction(nameof(Details), new { id = nuevaOrden.Id });
            }
            catch (Exception ex) when (ex is DuplicateOrderNumberException || 
                                       ex is ImportadorNotFoundException || 
                                       ex is InactiveImportadorException || 
                                       ex is ProveedorNotFoundException || 
                                       ex is InactiveProveedorException || 
                                       ex is PaisNotFoundException || 
                                       ex is InactivePaisException || 
                                       ex is CurrencyNotFoundException || 
                                       ex is InactiveCurrencyException ||
                                       ex is OrdenImportacionException)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarCatalogosEnViewBag();
                return View(viewModel);
            }
        }

    // GET: OrdenImportacion/Editar/5
    public async Task<IActionResult> Editar(int id)
    {
        var o = await _service.ObtenerPorIdAsync(id);
        if (o == null) return NotFound();

        var viewModel = new OrdenImportacionFormViewModel
        {
            Id = o.Id,
            NumeroOrden = o.NumeroOrden,
            ImportadorId = o.ImportadorId,
            ProveedorId = o.ProveedorId,
            PaisOrigenId = o.PaisOrigenId,
            MonedaId = o.MonedaId,
            FechaOrden = o.FechaOrden,
            MedioTransporte = o.MedioTransporte,
            EstadoOrden = o.EstadoOrden
        };

        await CargarCatalogosEnViewBag(o);
        return View(viewModel);
    }

    // POST: OrdenImportacion/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, OrdenImportacionFormViewModel viewModel)
    {
        if (id != viewModel.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            var oActual = await _service.ObtenerPorIdAsync(id);
            await CargarCatalogosEnViewBag(oActual);
            return View(viewModel);
        }

        try
        {
            var dto = new OrdenImportacionDto
            {
                Id = viewModel.Id,
                NumeroOrden = viewModel.NumeroOrden,
                ImportadorId = viewModel.ImportadorId,
                ProveedorId = viewModel.ProveedorId,
                PaisOrigenId = viewModel.PaisOrigenId,
                MonedaId = viewModel.MonedaId,
                FechaOrden = viewModel.FechaOrden,
                MedioTransporte = viewModel.MedioTransporte,
                EstadoOrden = viewModel.EstadoOrden
            };

            await _service.EditarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (OrdenImportacionLockedException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var oActual = await _service.ObtenerPorIdAsync(id);
            await CargarCatalogosEnViewBag(oActual);
            return View(viewModel);
        }
        catch (Exception ex) when (ex is DuplicateOrderNumberException || 
                                   ex is ImportadorNotFoundException || 
                                   ex is InactiveImportadorException || 
                                   ex is ProveedorNotFoundException || 
                                   ex is InactiveProveedorException || 
                                   ex is PaisNotFoundException || 
                                   ex is InactivePaisException || 
                                   ex is CurrencyNotFoundException || 
                                   ex is InactiveCurrencyException ||
                                   ex is OrdenImportacionNotFoundException ||
                                   ex is OrdenImportacionException)
        {
            ModelState.AddModelError("", ex.Message);
            var oActual = await _service.ObtenerPorIdAsync(id);
            await CargarCatalogosEnViewBag(oActual);
            return View(viewModel);
        }
    }

    // GET: OrdenImportacion/Eliminar/5
    public async Task<IActionResult> Eliminar(int id)
    {
        var o = await _service.ObtenerPorIdAsync(id);
        if (o == null) return NotFound();

        var viewModel = new OrdenImportacionIndexViewModel
        {
            Id = o.Id,
            NumeroOrden = o.NumeroOrden,
            ImportadorNombre = o.ImportadorNombre,
            ProveedorNombre = o.ProveedorNombre,
            PaisOrigenNombre = o.PaisOrigenNombre,
            MonedaNombre = o.MonedaNombre,
            FechaOrden = o.FechaOrden,
            MedioTransporte = o.MedioTransporte,
            EstadoOrden = o.EstadoOrden,
            FobTotal = o.FobTotal,
            TotalEstimadoLandedCost = o.TotalEstimadoLandedCost
        };

        return View(viewModel);
    }

    // POST: OrdenImportacion/Eliminar/5
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (OrdenImportacionLockedException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Eliminar), new { id });
        }
    }

    private async Task CargarCatalogosEnViewBag(OrdenImportacionDto? ordenActual = null)
    {
        var importadores = await _importadorService.ObtenerTodosAsync();
        var proveedores = await _proveedorService.ObtenerTodosAsync();
        var paises = await _paisService.ObtenerTodosAsync();
        var monedas = await _monedaService.ObtenerTodasAsync();

        // Si es creación, solo mostrar activos. Si es edición, mostrar activos + el actual aunque esté inactivo (Regla PDF p. 80)
        if (ordenActual == null)
        {
            ViewBag.Importadores = new SelectList(importadores.Where(x => x.Estado), "Id", "NombreRazonSocial");
            ViewBag.Proveedores = new SelectList(proveedores.Where(x => x.Estado), "Id", "Nombre");
            ViewBag.Paises = new SelectList(paises.Where(x => x.Estado), "Id", "Nombre");
            ViewBag.Monedas = new SelectList(monedas.Where(x => x.Estado), "Id", "Nombre");
        }
        else
        {
            ViewBag.Importadores = new SelectList(importadores.Where(x => x.Estado || x.Id == ordenActual.ImportadorId), "Id", "NombreRazonSocial");
            ViewBag.Proveedores = new SelectList(proveedores.Where(x => x.Estado || x.Id == ordenActual.ProveedorId), "Id", "Nombre");
            ViewBag.Paises = new SelectList(paises.Where(x => x.Estado || x.Id == ordenActual.PaisOrigenId), "Id", "Nombre");
            ViewBag.Monedas = new SelectList(monedas.Where(x => x.Estado || x.Id == ordenActual.MonedaId), "Id", "Nombre");
        }

        ViewBag.MediosTransporte = new SelectList(Enum.GetValues(typeof(MedioTransporte)));
    }
}
