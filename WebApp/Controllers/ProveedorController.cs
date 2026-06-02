using Capa_de_Datos.Repositories.Implementations;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Capa_de_Negocio.Exceptions;

namespace WebApp.Controllers;

public class ProveedorController : Controller
{
    private readonly IProveedorService _proveedorService;
    private readonly IPaisService _paisService;
    private readonly IMonedaService _monedaService;

    public ProveedorController(IProveedorService proveedorService, IPaisService paisService, IMonedaService monedaService)
    {
        _proveedorService = proveedorService;
        _paisService = paisService;
        _monedaService = monedaService;
    }

    // pantalla principal, Muestra la tabla con todos los proveedores
    public async Task<IActionResult> Index()
    {
        var proveedoresDto = await _proveedorService.ObtenerTodosAsync();
        return View(proveedoresDto);
    }

    // formulario de crear (get)
    public async Task<IActionResult> Crear()
    {
        var viewModel = new ProveedorCrearViewModel();
        await CargarCatalogosAsync(viewModel);
        return View(viewModel);
    }

    // procesa creacion (post)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ProveedorCrearViewModel model)
    {
        if (ModelState.IsValid)
        {
            var nuevoProveedorDto = new ProveedorDTO
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Telefono = model.Telefono,
                PaisId = model.PaisId,
                MonedaPrincipalId = model.MonedaPrincipalId,
                Estado = true
            };
            await _proveedorService.CrearAsync(nuevoProveedorDto);
            TempData["Success"] = "¡Proveedor registrado con éxito!";
            return RedirectToAction(nameof(Index));
        }

        await CargarCatalogosAsync(model);
        return View(model);
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var proveedorDto = await _proveedorService.ObtenerPorIdAsync(id);
        if (proveedorDto == null) return NotFound();

        var viewModel = new ProveedorCrearViewModel 
        {
            Id = proveedorDto.Id,
            Nombre = proveedorDto.Nombre,
            Correo = proveedorDto.Correo,
            Telefono = proveedorDto.Telefono,
            PaisId = proveedorDto.PaisId,
            MonedaPrincipalId = proveedorDto.MonedaPrincipalId,
            Estado = proveedorDto.Estado
        };

        await CargarCatalogosAsync(viewModel);
        return View(viewModel);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ProveedorCrearViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var proveedorDto = new ProveedorDTO
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Telefono = model.Telefono,
                    PaisId = model.PaisId,
                    MonedaPrincipalId = model.MonedaPrincipalId,
                    Estado = model.Estado
                };

                await _proveedorService.EditarAsync(proveedorDto);
                TempData["Success"] = "¡Proveedor actualizado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglasProvException ex) 
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el proveedor.");
            }
        }

        await CargarCatalogosAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var proveedorDto = await _proveedorService.ObtenerPorIdAsync(id);
        if (proveedorDto == null) return NotFound();

        return View(proveedorDto);
    }

    // ELIMINAR POST
    [HttpPost, ActionName("EliminarConfirmado")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        try
        {
            await _proveedorService.EliminarAsync(id);
            TempData["Success"] = "¡Proveedor eliminado con éxito!";
            return RedirectToAction(nameof(Index));
        }
        catch (ReglasProvException ex)
        {
            var proveedorDto = await _proveedorService.ObtenerPorIdAsync(id);
            if (proveedorDto == null) return NotFound();

            ViewBag.Error = ex.Message; // Pasa el mensaje de validación de ordenes
            return View("Eliminar", proveedorDto); // se queda en la misma vista enseñando el error
           // TempData["Error"] = ex.Message;
          //  return RedirectToAction(nameof(Eliminar), new { id });
        }
        catch (Exception)
        {
            var proveedorDto = await _proveedorService.ObtenerPorIdAsync(id);
            if (proveedorDto == null) return NotFound();

            ViewBag.Error = "No se pudo eliminar el proveedor debido a un error inesperado en el sistema.";
            return View("Eliminar", proveedorDto);
           // TempData["Error"] = "No se pudo eliminar el proveedor debido a un error inesperado.";
           // return RedirectToAction(nameof(Eliminar), new { id });
        }
    }

    private async Task CargarCatalogosAsync(ProveedorCrearViewModel model)
    {
        var paises = await _paisService.ObtenerTodosAsync();
        var monedas = await _monedaService.ObtenerTodasAsync();

        model.Paises = paises
            .Where(p => p.Estado || p.Id == model.PaisId)
            .Select(p => new KeyValuePair<string, string>(p.Id.ToString(), p.Nombre))
            .ToList();

        model.Monedas = monedas
            .Where(m => m.Estado || m.Id == model.MonedaPrincipalId)
            .Select(m => new KeyValuePair<string, string>(m.Id.ToString(), m.Nombre))
            .ToList();
    }
}
