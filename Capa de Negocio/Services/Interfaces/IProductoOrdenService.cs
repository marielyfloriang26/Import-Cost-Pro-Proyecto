using System.Collections.Generic;
using System.Threading.Tasks;
using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces
{
    public interface IProductoOrdenService
    {
        Task<OrdenResumenFobDto> ObtenerResumenOrdenAsync(int ordenId);
        Task<ProductoOrdenDto?> ObtenerPorIdAsync(int id);
        Task AgregarProductoAOrdenAsync(ProductoOrdenDto dto);
        Task EditarProductoEnOrdenAsync(ProductoOrdenDto dto);
        Task EliminarProductoDeOrdenAsync(int id);
        
        // NUEVO MÉTODO: Para alimentar el Select del formulario sin usar repositorios en la Web
        Task<IEnumerable<ProductoOrdenDto>> ObtenerProductosActivosAsync();
    }
}