using Capa_de_Datos.Repositories.Implementations;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            
            // Le manda esa lista directo a la Vista para que Bootstrap la dibuje
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

        // metodo para no repetir codigo al cargar los Dropdowns
        private async Task CargarDesplegablesAsync(ProveedorCrearViewModel model)
    {
        var listaPaises = await _paisRepository.GetAllAsync();
        var listaMonedas = await _monedaRepository.GetAllAsync();

        // Convierte las entidades a pares de Id y Nombre en texto plano
    model.Paises = listaPaises.Select(p => new KeyValuePair<string, string>(p.Id.ToString(), p.Nombre)).ToList();
    model.Monedas = listaMonedas.Select(m => new KeyValuePair<string, string>(m.Id.ToString(), m.Nombre)).ToList();
    }
}
