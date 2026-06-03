using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;
    public class CategoriasArancelariasController : Controller
    {
        private readonly ICategoriaArancelariaService _categoriaService;

        // Constructor
        public CategoriasArancelariasController(ICategoriaArancelariaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // Muestra el listado general, Pantalla Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categoriasDto = await _categoriaService.ObtenerTodasAsync();

            // Mapea los DTOs al ViewModel para la tabla HTML
            var viewModelList = categoriasDto.Select(c => new CategoriaArancelariaViewModel
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Descripcion = c.Descripcion,
                PorcentajeArancel = c.PorcentajeArancel,
                AplicaItbis = c.AplicaItbis,
                AplicaSelectivo = c.AplicaSelectivo,
                PorcentajeSelectivo = c.PorcentajeSelectivo,
                Estado = c.Estado,
                TieneProductosAsociados = c.TieneProductosAsociados
            }).ToList();

            return View(viewModelList);
        }

        // Muestra el formulario de creacion vacio 
        [HttpGet]
        public IActionResult Crear()
        {
            var model = new CategoriaArancelariaViewModel
            {
                Estado = true // Por defecto activa 
            };
            return View(model);
        }

        // Recibe los datos e intenta guardar la nueva categoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CategoriaArancelariaViewModel model)
        {
            // Regla: Si no aplica selectivo, el porcentaje debe ser 0
            if (!model.AplicaSelectivo)
            {
                model.PorcentajeSelectivo = 0;
                ModelState.Remove(nameof(model.PorcentajeSelectivo)); // Limpia errores residuales de validacion
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var dto = new CategoriaArancelariaDTO
                {
                    Codigo = model.Codigo,
                    Descripcion = model.Descripcion,
                    PorcentajeArancel = model.PorcentajeArancel,
                    AplicaItbis = model.AplicaItbis,
                    AplicaSelectivo = model.AplicaSelectivo,
                    PorcentajeSelectivo = model.PorcentajeSelectivo
                };

                await _categoriaService.CrearAsync(dto);

                TempData["SuccessMessage"] = "Categoría arancelaria registrada de manera exitosa.";
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

            return View(model);
        }

        // Muestra el formulario de edicion con datos cargados 
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _categoriaService.ObtenerPorIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            var model = new CategoriaArancelariaViewModel
            {
                Id = dto.Id,
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                PorcentajeArancel = dto.PorcentajeArancel,
                AplicaItbis = dto.AplicaItbis,
                AplicaSelectivo = dto.AplicaSelectivo,
                PorcentajeSelectivo = dto.PorcentajeSelectivo,
                Estado = dto.Estado,
                TieneProductosAsociados = dto.TieneProductosAsociados // Le avisa a la vista si debe bloquear inputs
            };

            return View(model);
        }

        // Recibe los cambios e intenta actualizar la categoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CategoriaArancelariaViewModel model)
        {
            // Busca el registro real actual directamente desde la base de datos para comparar sus valores
            var categoriaOriginal = await _categoriaService.ObtenerPorIdAsync(model.Id);
            if (categoriaOriginal == null)
            {
                return NotFound();
            }

            // Evalua si la categoria original de la base de datos ya cuenta con productos amarrados
            if (categoriaOriginal.TieneProductosAsociados)
            {
            
               if( model.Codigo != categoriaOriginal.Codigo ||
                model.PorcentajeArancel != categoriaOriginal.PorcentajeArancel ||
                model.AplicaItbis != categoriaOriginal.AplicaItbis ||
                model.AplicaSelectivo != categoriaOriginal.AplicaSelectivo ||
                model.PorcentajeSelectivo != categoriaOriginal.PorcentajeSelectivo)
            {
                // Si cambio algo, registram el error en el ModelState. Esto detiene el flujo de guardado.
                    ModelState.AddModelError(string.Empty, "No se permite modificar estos parámetros (Código, Aranceles o Impuestos) en categorías que ya tienen productos asociados para proteger el histórico de cálculos.");
                    
                    // Forza la propiedad de control en true para que la vista mantenga los inputs bloqueados al recargar.
                    model.TieneProductosAsociados = true;
                    return View(model);
            }
              /*  ModelState.Remove(nameof(model.Codigo));
                ModelState.Remove(nameof(model.PorcentajeArancel));
                ModelState.Remove(nameof(model.PorcentajeSelectivo)); */
            }
            else
            {

            if (!model.AplicaSelectivo)
            {
                model.PorcentajeSelectivo = 0;
                ModelState.Remove(nameof(model.PorcentajeSelectivo));
            }
            }
            if (!ModelState.IsValid)
            {
                // agg
                model.TieneProductosAsociados = categoriaOriginal.TieneProductosAsociados;
                return View(model);
            }

            try
            {
                var dto = new CategoriaArancelariaDTO
                {
                    Id = model.Id,
                    Codigo = model.Codigo,
                    Descripcion = model.Descripcion,
                    PorcentajeArancel = model.PorcentajeArancel,
                    AplicaItbis = model.AplicaItbis,
                    AplicaSelectivo = model.AplicaSelectivo,
                    PorcentajeSelectivo = model.PorcentajeSelectivo,
                    Estado = model.Estado
                };

                await _categoriaService.EditarAsync(dto);

                TempData["SuccessMessage"] = "Categoría arancelaria actualizada de manera exitosa.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar la categoría arancelaria.");
            }

            return View(model);
        }

        [HttpGet]
public async Task<IActionResult> Eliminar(int id)
{
    // Buscas la categoría en tu capa de negocio usando el id
    var dto = await _categoriaService.ObtenerPorIdAsync(id); 
    
    if (dto == null)
    {
        TempData["ErrorMessage"] = "La categoría arancelaria no existe.";
        return RedirectToAction(nameof(Index));
    }

    // Mapea el DTO al ViewModel que la vista espera recibir
            var model = new CategoriaArancelariaViewModel
            {
                Id = dto.Id,
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                PorcentajeArancel = dto.PorcentajeArancel,
                AplicaItbis = dto.AplicaItbis,
                AplicaSelectivo = dto.AplicaSelectivo,
                PorcentajeSelectivo = dto.PorcentajeSelectivo,
                Estado = dto.Estado
            };

            return View(model);
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(CategoriaArancelariaViewModel model)
        {
            try
            {
                await _categoriaService.EliminarAsync(model.Id);
                TempData["SuccessMessage"] = "Categoría arancelaria eliminada de manera exitosa.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                // Almacena el mensaje y se queda en la misma vista para mostrar la advertencia
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la categoría arancelaria.";
            }

            // Regresa a la vista de confirmación pasándole el modelo para que no se vacíen los campos visibles
            return RedirectToAction(nameof(Index));
        }
    }
