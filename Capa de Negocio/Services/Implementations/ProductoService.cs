using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.Exceptions;

namespace Capa_de_Negocio.Implementations;
    public class ProductoService : IProductoService
    {
        private readonly IRepository<Producto> _productoRepo;
        private readonly IRepository<Pais> _paisRepo;
        private readonly IRepository<CategoriaArancelaria> _categoriaRepo;

        // Constructor, Inyecta los repositorios necesarios
        public ProductoService(IRepository<Producto> productoRepo, IRepository<Pais> paisRepo, IRepository<CategoriaArancelaria> categoriaRepo)
        {
            _productoRepo = productoRepo;
            _paisRepo = paisRepo;
            _categoriaRepo = categoriaRepo;
        }

        // Obtener todos los productos para la pantalla inicial (Index)
        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync()
        {
            // Busca todos los productos en la bd (incluyendo Pais y Categoria via Repo)
            var productos = await _productoRepo.GetAllAsync();

            // Mapea la lista de Entidades fisicas a la lista de DTOs tradicionales
            var listaDto = productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoReferencia = p.CodigoReferencia,
                PaisOrigenId = p.PaisOrigenId,
                // Usa la propiedad de navegacion ya cargada
                PaisOrigenNombre = p.PaisOrigen?.Nombre ?? "No asignado",
                
                CategoriaId = p.CategoriaId,
                // Usa la propiedad de navegacion ya cargada
                CategoriaNombre = p.Categoria?.Descripcion ?? "Sin categoría",
                
                PesoUnitario = p.PesoUnitario,
                Largo = p.Largo,
                Ancho = p.Ancho,
                Alto = p.Alto,
                UnidadMedida = p.UnidadMedida,
                Descripcion = p.Descripcion,
                Estado = p.Estado
            }).ToList();

            return listaDto;
        }

       // Verifica si un codigo ya existe (ignorando espacios al inicio/final)
        public async Task<bool> ExisteCodigoAsync(string codigo, int idExcluir = 0)
        {
            // regla: ignorar espacios limpiando el parametro con Trim()
            if (string.IsNullOrWhiteSpace(codigo)) return false;
            string codigoLimpio = codigo.Trim().ToLower();

            // Busca en la bd si algun producto coincide con ese codigo
            var productosConMismoCodigo = await _productoRepo.FindAsync(p => 
                p.CodigoReferencia.Trim().ToLower() == codigoLimpio && 
                p.Id != idExcluir // Si esta editando, ignora el producto actual
            );

            // Si encuentra al menos uno, significa que el codigo ya existe
            return productosConMismoCodigo.Any();
        }

        // Obtiene un solo producto por su ID para cargar el formulario de edicion
        public async Task<ProductoDTO?> ObtenerPorIdAsync(int id)
        {
            // busca el producto en la bd usando el ID (incluyendo relaciones)
            var producto = await _productoRepo.GetByIdAsync(id);

            // Si no se encuentra, devuelve null de forma segura
            if (producto == null) return null;

            // Mapea la entidad de la base de datos al DTO tradicional
            return new ProductoDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                CodigoReferencia = producto.CodigoReferencia,
                PaisOrigenId = producto.PaisOrigenId,
                PaisOrigenNombre = producto.PaisOrigen?.Nombre ?? "No asignado",
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = producto.Categoria?.Descripcion ?? "Sin categoría",
                PesoUnitario = producto.PesoUnitario,
                Largo = producto.Largo,
                Ancho = producto.Ancho,
                Alto = producto.Alto,
                UnidadMedida = producto.UnidadMedida,
                Descripcion = producto.Descripcion,
                Estado = producto.Estado
            };
        }
        // Registra un nuevo producto con validaciones estrictas
        public async Task<bool> CrearAsync(ProductoDTO dto)
        {
            // Aplica la regla de limpiar espacios al inicio y al final
            dto.Nombre = dto.Nombre.Trim();
            dto.CodigoReferencia = dto.CodigoReferencia.Trim();

            // Regla: Validar si el codigo ya existe
            if (await ExisteCodigoAsync(dto.CodigoReferencia))
            {
                throw new ReglasProductoException("Ya existe un producto registrado con este código o referencia.");
            }

            // Regla: Si se coloca largo, ancho o alto, los tres deben tener valor y ser mayores a 0
            bool tieneLargo = dto.Largo.HasValue;
            bool tieneAncho = dto.Ancho.HasValue;
            bool tieneAlto = dto.Alto.HasValue;

            // Si al menos uno tiene valor, pero no los tres, hay un error
            if (tieneLargo || tieneAncho || tieneAlto)
            {
                if (!tieneLargo || !tieneAncho || !tieneAlto)
                {
                    throw new ReglasProductoException("Si se especifica una dimensión física (Largo, Ancho o Alto), se deben colocar las tres obligatoriamente.");
                }

                // Si estan los tres, valida que sean mayores que 0
                if (dto.Largo <= 0 || dto.Ancho <= 0 || dto.Alto <= 0)
                {
                    throw new ReglasProductoException("Los valores de Largo, Ancho y Alto deben ser mayores que 0.");
                }
            }

            // Si paso todas las validaciones, mapea el DTO a la Entidad fisica
            var nuevoProducto = new Producto
            {
                Nombre = dto.Nombre,
                CodigoReferencia = dto.CodigoReferencia,
                PaisOrigenId = dto.PaisOrigenId,
                CategoriaId = dto.CategoriaId, // !
                PesoUnitario = dto.PesoUnitario,
                Largo = dto.Largo,
                Ancho = dto.Ancho,
                Alto = dto.Alto,
                UnidadMedida = dto.UnidadMedida, // Mapea directo a Enum
                Descripcion = dto.Descripcion?.Trim(),
                Estado = true // Por defecto activo al crear
            };

            // Guara en la base de datos usando el repositorio
            await _productoRepo.AddAsync(nuevoProducto);
            await _productoRepo.SaveAsync();

            return true;
        }

        // Guarda cambios de un producto existente
        public async Task<bool> EditarAsync(ProductoDTO dto)
        {
            // Busca el producto existente en la base de datos
            var productoExistente = await _productoRepo.GetByIdAsync(dto.Id);
            if (productoExistente == null)
            {
                throw new ReglasProductoException("El producto que intenta editar no existe en el sistema.");
            }

            // Limpia los espacios en blanco 
            dto.Nombre = dto.Nombre.Trim();
            dto.CodigoReferencia = dto.CodigoReferencia.Trim();

            // Regla: Valida si el codigo ya existe en otro producto, pasando el dto.Id para excluirlo
            if (await ExisteCodigoAsync(dto.CodigoReferencia, dto.Id))
            {
                throw new ReglasProductoException("Ya existe un producto registrado con este código o referencia.");
            }

            // Regla: Validar dimensiones dependientes (Largo, Ancho, Alto)
            bool tieneLargo = dto.Largo.HasValue;
            bool tieneAncho = dto.Ancho.HasValue;
            bool tieneAlto = dto.Alto.HasValue;

            if (tieneLargo || tieneAncho || tieneAlto)
            {
                if (!tieneLargo || !tieneAncho || !tieneAlto)
                {
                    throw new ReglasProductoException("Si se especifica una dimensión física (Largo, Ancho o Alto), se deben colocar las tres obligatoriamente.");
                }

                if (dto.Largo <= 0 || dto.Ancho <= 0 || dto.Alto <= 0)
                {
                    throw new ReglasProductoException("Los valores de Largo, Ancho y Alto deben ser mayores que 0.");
                }
            }

            // Actualiza los campos de la entidad con los datos del DTO
            productoExistente.Nombre = dto.Nombre;
            productoExistente.CodigoReferencia = dto.CodigoReferencia;
            productoExistente.PaisOrigenId = dto.PaisOrigenId;

            // !!!
           productoExistente.CategoriaId = dto.CategoriaId;
            productoExistente.PesoUnitario = dto.PesoUnitario;
            productoExistente.Largo = dto.Largo;
            productoExistente.Ancho = dto.Ancho;
            productoExistente.Alto = dto.Alto;
            productoExistente.UnidadMedida = dto.UnidadMedida;
            productoExistente.Descripcion = dto.Descripcion?.Trim();
            productoExistente.Estado = dto.Estado; // En la edicion el usuario si puede cambiar el estado

            // Guarda los cambios en la base de datos
            _productoRepo.Update(productoExistente);
            await _productoRepo.SaveAsync();

            return true;
        }
        // Elimina un producto si no esta amarrado a ninguna orden de importacion
        public async Task<bool> EliminarAsync(int id)
        {
            // Busca el producto en la base de datos
            var producto = await _productoRepo.GetByIdAsync(id);
            if (producto == null)
            {
                throw new ReglasProductoException("El producto que intenta eliminar no existe.");
            }

            // Regla: Valida si esta asociado a ordenes de importación
            if (producto.ProductosOrden != null && producto.ProductosOrden.Any())
            {
                throw new ReglasProductoException("No se puede eliminar este producto porque está asociado a una o más órdenes de importación.");
            }

            // Si esta limpio y no tiene registros historicos, se elimina
            _productoRepo.Remove(producto);
            await _productoRepo.SaveAsync();

            return true;
        }
    }
