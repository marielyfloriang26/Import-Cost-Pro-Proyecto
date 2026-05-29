using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;

namespace Capa_Negocio.Implementations
{
    public class CategoriaArancelariaService : ICategoriaArancelariaService
    {
        private readonly IRepository<CategoriaArancelaria> _categoriaRepo;
        private readonly IRepository<Producto> _productoRepo;

        // Constructor 
        public CategoriaArancelariaService(
            IRepository<CategoriaArancelaria> categoriaRepo, 
            IRepository<Producto> productoRepo)
        {
            _categoriaRepo = categoriaRepo;
            _productoRepo = productoRepo;
        }

    
        public async Task<IEnumerable<CategoriaArancelariaDTO>> ObtenerTodasAsync()
        {
            var categorias = await _categoriaRepo.GetAllAsync();

            return categorias.Select(c => new CategoriaArancelariaDTO
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Descripcion = c.Descripcion,
                PorcentajeArancel = c.PorcentajeArancel,
                AplicaItbis = c.AplicaItbis,
                AplicaSelectivo = c.AplicaSelectivo,
                PorcentajeSelectivo = c.PorcentajeSelectivo,
                Estado = c.Estado,
                // Evalua si tiene productos vinculados en la coleccion de la entidad
                TieneProductosAsociados = c.Productos != null && c.Productos.Any()
            }).ToList();
        }

        // Para el select de Productos, Retorna solo las activas
        public async Task<IEnumerable<CategoriaArancelariaDTO>> ObtenerActivasAsync()
        {
            // Busca las categorias cuyo Estado sea true
            var categoriasActivas = await _categoriaRepo.FindAsync(c => c.Estado == true);

            return categoriasActivas.Select(c => new CategoriaArancelariaDTO
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Descripcion = c.Descripcion,
                PorcentajeArancel = c.PorcentajeArancel,
                AplicaItbis = c.AplicaItbis,
                AplicaSelectivo = c.AplicaSelectivo,
                PorcentajeSelectivo = c.PorcentajeSelectivo,
                Estado = c.Estado
            }).ToList();
        }

        // Obtener una sola categoria por su ID
        public async Task<CategoriaArancelariaDTO?> ObtenerPorIdAsync(int id)
        {
            var c = await _categoriaRepo.GetByIdAsync(id);
            if (c == null) return null;

            return new CategoriaArancelariaDTO
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Descripcion = c.Descripcion,
                PorcentajeArancel = c.PorcentajeArancel,
                AplicaItbis = c.AplicaItbis,
                AplicaSelectivo = c.AplicaSelectivo,
                PorcentajeSelectivo = c.PorcentajeSelectivo,
                Estado = c.Estado,
                TieneProductosAsociados = c.Productos != null && c.Productos.Any()
            };
        }

        // Validar si el codigo ya existe (Ignorando espacios)
        public async Task<bool> ExisteCodigoAsync(string codigo, int idExcluir = 0)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return false;
            string codigoLimpio = codigo.Trim().ToLower();

            var duplicados = await _categoriaRepo.FindAsync(c => 
                c.Codigo.Trim().ToLower() == codigoLimpio && 
                c.Id != idExcluir
            );

            return duplicados.Any();
        }

        public async Task<bool> CrearAsync(CategoriaArancelariaDTO dto)
        {
            // Regla: Limpiar espacios al inicio y final
            dto.Codigo = dto.Codigo.Trim();
            dto.Descripcion = dto.Descripcion.Trim();

            // Regla: Validar unicidad del código
            if (await ExisteCodigoAsync(dto.Codigo))
            {
                throw new CatArancelariaException("Ya existe una categoría arancelaria registrada con este código.");
            }

            // Validar limites de los porcentajes
            ValidarPorcentajes(dto);

            // Mapeo a la entidad fisica
            var nuevaCategoria = new CategoriaArancelaria
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                PorcentajeArancel = dto.PorcentajeArancel,
                AplicaItbis = dto.AplicaItbis,
                AplicaSelectivo = dto.AplicaSelectivo,
                PorcentajeSelectivo = dto.AplicaSelectivo ? dto.PorcentajeSelectivo : 0, // Fuerza 0 si es No
                Estado = true // Por defecto viene activa segun el mandato
            };

            await _categoriaRepo.AddAsync(nuevaCategoria);
            await _categoriaRepo.SaveAsync();

            return true;
        }

        // Editar una categoria existente
        public async Task<bool> EditarAsync(CategoriaArancelariaDTO dto)
        {
            var categoriaExistente = await _categoriaRepo.GetByIdAsync(dto.Id);
            if (categoriaExistente == null)
            {
                throw new CatArancelariaException("La categoría arancelaria que intenta editar no existe.");
            }

            dto.Codigo = dto.Codigo.Trim();
            dto.Descripcion = dto.Descripcion.Trim();

            // Regla: Validar unicidad excluyendo la actual
            if (await ExisteCodigoAsync(dto.Codigo, dto.Id))
            {
                throw new CatArancelariaException("Ya existe una categoría arancelaria registrada con este código.");
            }

            ValidarPorcentajes(dto);

            // Verificar si tiene productos asociados para aplicar el bloqueo critico
            bool tieneProductos = categoriaExistente.Productos != null && categoriaExistente.Productos.Any();

            if (tieneProductos)
            {
                // Regla: Si ya tiene productos, no se permiten alterar campos de calculo
                if (categoriaExistente.Codigo != dto.Codigo ||
                    categoriaExistente.PorcentajeArancel != dto.PorcentajeArancel ||
                    categoriaExistente.AplicaItbis != dto.AplicaItbis ||
                    categoriaExistente.AplicaSelectivo != dto.AplicaSelectivo ||
                    categoriaExistente.PorcentajeSelectivo != (dto.AplicaSelectivo ? dto.PorcentajeSelectivo : 0))
                {
                    throw new CatArancelariaException("No se pueden modificar los campos críticos (Código, Arancel o Impuestos) porque esta categoría ya está asociada a productos registrados. Desactívela si no desea usarla más.");
                }
            }
            else
            {
                // Si esta limpia, se permite modificar todo
                categoriaExistente.Codigo = dto.Codigo;
                categoriaExistente.PorcentajeArancel = dto.PorcentajeArancel;
                categoriaExistente.AplicaItbis = dto.AplicaItbis;
                categoriaExistente.AplicaSelectivo = dto.AplicaSelectivo;
                categoriaExistente.PorcentajeSelectivo = dto.AplicaSelectivo ? dto.PorcentajeSelectivo : 0;
            }

            // Datos comunes no criticos que siempre se pueden editar
            categoriaExistente.Descripcion = dto.Descripcion;
            categoriaExistente.Estado = dto.Estado;

            _categoriaRepo.Update(categoriaExistente);
            await _categoriaRepo.SaveAsync();

            return true;
        }

        // Eliminar la categoria arancelaria
        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _categoriaRepo.GetByIdAsync(id);
            if (categoria == null)
            {
                throw new CatArancelariaException("La categoría arancelaria que intenta eliminar no existe.");
            }

            // Regla: No eliminar si esta amarrada a productos
            if (categoria.Productos != null && categoria.Productos.Any())
            {
                throw new CatArancelariaException("No se puede eliminar esta categoría arancelaria porque está asociada a productos registrados.");
            }

            _categoriaRepo.Remove(categoria);
            await _categoriaRepo.SaveAsync();

            return true;
        }

        // Método auxiliar para validar aranceles e impuestos
        private void ValidarPorcentajes(CategoriaArancelariaDTO dto)
        {
            // Validaciones de Arancel
            if (dto.PorcentajeArancel < 0 || dto.PorcentajeArancel > 100)
            {
                throw new CatArancelariaException("El porcentaje de arancel debe estar entre 0 y 100.");
            }

            // Validaciones del Impuesto Selectivo
            if (dto.AplicaSelectivo)
            {
                if (dto.PorcentajeSelectivo <= 0 || dto.PorcentajeSelectivo > 100)
                {
                    throw new CatArancelariaException("Si aplica impuesto selectivo, el porcentaje debe ser requerido y mayor que 0, hasta un máximo de 100.");
                }
            }
        }
    }
}