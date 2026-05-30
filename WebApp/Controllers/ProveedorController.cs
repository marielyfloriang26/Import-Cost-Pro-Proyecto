using Capa_de_Datos.Repositories.Implementations;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Capa_de_Negocio.Exceptions;

namespace WebApp.Controllers;
    public class ProveedoresController : Controller
    {
        private readonly IProveedorService _proveedorService;
        private readonly IRepository<Capa_de_Datos.Entities.Pais> _paisRepository;
        private readonly IRepository<Capa_de_Datos.Entities.Moneda> _monedaRepository;

        // El constructor recibe el servicio de negocio para usarlo en las pantallas
        public ProveedoresController(IProveedorService proveedorService, IRepository<Capa_de_Datos.Entities.Pais> paisRepository, IRepository<Capa_de_Datos.Entities.Moneda> monedaRepository)
        {
            _proveedorService = proveedorService;
            _paisRepository = paisRepository;
            _monedaRepository = monedaRepository;
        }

        // pantalla principal, Muestra la tabla con todos los proveedores
        public async Task<IActionResult> Index()
        {
            // Llama al servicio para traer la lista de proveedores limpia (DTOs)
            var proveedoresDto = await _proveedorService.ObtenerTodosAsync();

            return View(proveedoresDto);
        }

        // formulario de crear (get)
        public async Task<IActionResult> Crear()
    {
        var viewModel = new ProveedorCrearViewModel();
        await CargarDesplegablesAsync(viewModel); // llena los combos de paises y monedas
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

        // si el formulario no es valido, recarga los combo y muestra los errores 
        await CargarDesplegablesAsync(model);
        return View(model);
    }

// GET
[HttpGet]
public async Task<IActionResult> Editar(int id)
{
    // Buscamos el proveedor real en la BD
    var proveedorDto = await _proveedorService.ObtenerPorIdAsync(id);
    if (proveedorDto == null) return NotFound();

    // Pasamos los datos al ViewModel limpio que usa la vista
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

    await CargarDesplegablesAsync(viewModel); // Llena los combos de paises y monedas
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
            TempData["Error"] = ex.Message; //No cambiar pais ni moneda si tiene ordenes
        }
        catch (Exception)
        {
            TempData["Error"] = "Ocurrió un error inesperado al actualizar el proveedor.";
        }
    }

    await CargarDesplegablesAsync(model);
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
    [HttpPost]
    [ActionName("EliminarConfirmado")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        try
        {
            await _proveedorService.EliminarAsync(id);
            TempData["Success"] = "¡Proveedor eliminado con éxito!";
        }
        catch (ReglasProvException ex)
        {
            TempData["Error"] = ex.Message; 
        }
        catch (Exception)
        {
            TempData["Error"] = "No se pudo eliminar el proveedor debido a un error inesperado.";
        }

        return RedirectToAction(nameof(Index));
    }

        private async Task CargarDesplegablesAsync(ProveedorCrearViewModel model)
    {
        // busca lo que hay en la bd 
        var listaPaises = await _paisRepository.GetAllAsync();
        var listaMonedas = await _monedaRepository.GetAllAsync();

        // Si es una creación, filtra solo los activos
        // Si es una edición, incluye el actual aunque esté inactivo para preservar el historial 
        // Convierte las entidades a pares de Id y Nombre en texto plano
        model.Paises = listaPaises!
            .Where(p => p.Estado || p.Id == model.PaisId)
            .Select(p => new KeyValuePair<string, string>(p.Id.ToString(), p.Nombre))
            .ToList();

        model.Monedas = listaMonedas!
            .Where(m => m.Estado || m.Id == model.MonedaPrincipalId)
            .Select(m => new KeyValuePair<string, string>(m.Id.ToString(), m.Nombre))
            .ToList();
    }
}
