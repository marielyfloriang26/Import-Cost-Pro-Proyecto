using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Capa_de_Negocio.ViewModels;


namespace WebApp.Controllers;
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IPaisService _paisService; // Inyecta el servicio de paises para los Dropdowns
        private readonly ICategoriaArancelariaService _categoriaService;

        // Constructor
        public ProductosController(IProductoService productoService, IPaisService paisService, ICategoriaArancelariaService categoriaService)
        {
            _productoService = productoService;
            _paisService = paisService;
            _categoriaService = categoriaService;
        }

        // Muestra el listado general de productos (Pantalla Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Llama al servicio para obtener todos los productos procesados
            var productosDto = await _productoService.ObtenerTodosAsync();

            // Mapea la lista de DTOs a una lista de ViewModels para la vista HTML
            var viewModelList = productosDto.Select(p => new ProductoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoReferencia = p.CodigoReferencia,
                PaisOrigenNombre = p.PaisOrigenNombre,
                CategoriaNombre = p.CategoriaNombre,
                PesoUnitario = p.PesoUnitario,
                UnidadMedidaSelected = p.UnidadMedida.ToString(), // Convierte el Enum a Texto para la pantalla
                Estado = p.Estado,

                Largo = p.Largo,
                Ancho = p.Ancho,
                Alto = p.Alto
            }).ToList();

            // Envia la lista armada a la vista Index.cshtml
            return View(viewModelList);
        }

        // Muestra el formulario de creacion vacio (Pantalla Crear)
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var model = new ProductoViewModel();
            
            // Llena las opciones de paises para el combo-box
            await CargarOpcionesFormulario(model, 0);

            return View(model);
        }

        // Recibir los datos del formulario e intentar guardar el producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoViewModel model)
        {
            // Si las validaciones basicas de la pantalla fallan 
            if (!ModelState.IsValid)
            {
                await CargarOpcionesFormulario(model, model.CategoriaId); // Recarga los combos para no perderlos
                return View(model);
            }

            try
            {
                // Mapea el ViewModel al DTO que espera la Capa de Negocio
                var dto = new ProductoDTO
                {
                    Nombre = model.Nombre,
                    CodigoReferencia = model.CodigoReferencia,
                    PaisOrigenId = model.PaisOrigenId,
                    CategoriaId = model.CategoriaId,
                    PesoUnitario = model.PesoUnitario,
                    Largo = model.Largo,
                    Ancho = model.Ancho,
                    Alto = model.Alto,
                    Descripcion = model.Descripcion,
                    // Convierte de forma segura el string de la vista al Enum
                    UnidadMedida = Enum.Parse<Capa_de_Datos.Enums.UnidadMedida>(model.UnidadMedidaSelected)
                };

                // Llama al servicio para guardar con todas las reglas pesadas
                await _productoService.CrearAsync(dto);

                // Si todo sale bien, vuelve al listado principal con un mensaje de éxito
                TempData["SuccessMessage"] = "Producto registrado de manera exitosa.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex) 
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado en el sistema. Intente de nuevo.");
            }

            // Si llega aqui es porque hubo un error, recarga las opciones y vuelve a la vista
            await CargarOpcionesFormulario(model, model.CategoriaId);
            return View(model);
        }

        // Para no repetir codigo al cargar paises y categorias simuladas
        private async Task CargarOpcionesFormulario(ProductoViewModel model, int categoriaIdActual)
        {
            // Busca los paises registrados en la bd
            var paises = await _paisService.ObtenerTodosAsync(); 

            model.PaisesOptions = paises.Select(p => new KeyValuePair<string, string>(
                p.Id.ToString(), 
                p.Nombre
            )).ToList();

            // !!
            // busca las categorias arancelarias de la bd
            var categoriasA = (await _categoriaService.ObtenerActivasAsync()).ToList();

            if (categoriaIdActual > 0 && !categoriasA.Any(c => c.Id == categoriaIdActual))
            {
            var categoriaInactiva = await _categoriaService.ObtenerPorIdAsync(categoriaIdActual);
            if (categoriaInactiva !=null)
            {
                categoriaInactiva.Descripcion = $"{categoriaInactiva.Descripcion} (Inactiva)";
                categoriasA.Add(categoriaInactiva);
            }
            }
            // Mapea las opciones armando un texto
        model.CategoriasOptions = categoriasA.Select(c => new KeyValuePair<string, string>(
            c.Id.ToString(),
            $"{c.Codigo} - {c.Descripcion} ({c.PorcentajeArancel}%)"
        )).ToList();
        }

        // Mostrar el formulario de edicion con los datos precargados (Pantalla Editar)
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            // Busca el producto por su ID a través del servicio
            var dto = await _productoService.ObtenerPorIdAsync(id);
            if (dto == null)
            {
                return NotFound(); // Retorna un error 404 si el producto no existe
            }

            // Mapea los datos del DTO al ViewModel que entiende la pantalla
            var model = new ProductoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                CodigoReferencia = dto.CodigoReferencia,
                PaisOrigenId = dto.PaisOrigenId,
                CategoriaId = dto.CategoriaId,
                PesoUnitario = dto.PesoUnitario,
                Largo = dto.Largo,
                Ancho = dto.Ancho,
                Alto = dto.Alto,
                Descripcion = dto.Descripcion,
                UnidadMedidaSelected = dto.UnidadMedida.ToString(),
                Estado = dto.Estado
            };

            // Carga los Dropdowns de países y categorias para que aparezcan las opciones en el formulario
            await CargarOpcionesFormulario(model, dto.CategoriaId);

            return View(model);
        }

        // Recibe los datos modificados e intenta actualizar el producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProductoViewModel model)
        {
            // Si las validaciones del formulario web fallan
            if (!ModelState.IsValid)
            {
                await CargarOpcionesFormulario(model, model.CategoriaId); // Recarga las listas para no romper la pantalla
                return View(model);
            }

            try
            {
                // Mapea el ViewModel de vuelta al DTO para enviarlo a la capa de negocio
                var dto = new ProductoDTO
                {
                    Id = model.Id, // pasa el ID para que el sistema sepa que registro actualizar
                    Nombre = model.Nombre,
                    CodigoReferencia = model.CodigoReferencia,
                    PaisOrigenId = model.PaisOrigenId,
                    CategoriaId = model.CategoriaId,
                    PesoUnitario = model.PesoUnitario,
                    Largo = model.Largo,
                    Ancho = model.Ancho,
                    Alto = model.Alto,
                    Descripcion = model.Descripcion,
                    UnidadMedida = Enum.Parse<Capa_de_Datos.Enums.UnidadMedida>(model.UnidadMedidaSelected),
                    Estado = model.Estado // El usuario puede inactivar un producto en la edicion
                };

                // Ejecuta la edicion en la capa de negocio con todas las validaciones
                await _productoService.EditarAsync(dto);

                TempData["SuccessMessage"] = "Producto actualizado de manera exitosa.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex) 
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el producto.");
            }

            // Si llega aqui hubo un error, recarga combos y devuelve la vista
            await CargarOpcionesFormulario(model, model.CategoriaId);
            return View(model);
        }

        [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var dto = await _productoService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var model = new ProductoViewModel
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        CodigoReferencia = dto.CodigoReferencia,
        PaisOrigenNombre = dto.PaisOrigenNombre,
        CategoriaNombre = dto.CategoriaNombre,
        PesoUnitario = dto.PesoUnitario,
        UnidadMedidaSelected = dto.UnidadMedida.ToString(),
        Estado = dto.Estado
    };
        return View(model); 
    }

        // Elimina un producto de forma segura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                // Llama al servicio para intentar la eliminacion 
                await _productoService.EliminarAsync(id);

                // Si todo sale bien, manda un mensaje de exito al Index
                TempData["SuccessMessage"] = "Producto eliminado de manera exitosa.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex) 
            {
              //  TempData["ErrorMessage"] = ex.Message;
            var dto = await _productoService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

        var model = new ProductoViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            CodigoReferencia = dto.CodigoReferencia,
            PaisOrigenNombre = dto.PaisOrigenNombre,
            CategoriaNombre = dto.CategoriaNombre,
            PesoUnitario = dto.PesoUnitario,
            UnidadMedidaSelected = dto.UnidadMedida.ToString(),
            Estado = dto.Estado
        };

        // Pasa el mensaje de error especifico a la vista
        ViewBag.Error = ex.Message;
        return View("Eliminar", model);
            }
            catch (Exception)
            {
                var dto = await _productoService.ObtenerPorIdAsync(id);
                if (dto == null) return NotFound();

        var model = new ProductoViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            CodigoReferencia = dto.CodigoReferencia,
            PaisOrigenNombre = dto.PaisOrigenNombre,
            CategoriaNombre = dto.CategoriaNombre,
            PesoUnitario = dto.PesoUnitario,
            UnidadMedidaSelected = dto.UnidadMedida.ToString(),
            Estado = dto.Estado
        };
            
        ViewBag.Error = "No se puede eliminar este producto porque está asociado a una o más órdenes de importación.";
        return View("Eliminar", model);
               // TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el producto.";
            

            // Redirecciona siempre al listado principal para refrescar la tabla y ver las alertas
           // return RedirectToAction(nameof(Index));
        }
    }
    }
