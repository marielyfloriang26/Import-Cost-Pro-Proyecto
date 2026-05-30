using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces;
    public interface IProductoService
    {
        // Para la pantalla inicial, Muestra el listado de todos los productos registrados
        Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync();

        // Para cargar los datos actuales cuando se le de al boton Editar
        Task<ProductoDTO?> ObtenerPorIdAsync(int id);

        // Crear producto
        Task<bool> CrearAsync(ProductoDTO dto);

        // Para actualizar la informacion del producto seleccionado 
        Task<bool> EditarAsync(ProductoDTO dto);

        // Para quitar un producto del sistema, validando que no tenga ordenes asociadas
        Task<bool> EliminarAsync(int id);

        // validas si el codigo/referencia ya existe y no repetirlo
        Task<bool> ExisteCodigoAsync(string codigo, int idExcluir = 0);
    }
