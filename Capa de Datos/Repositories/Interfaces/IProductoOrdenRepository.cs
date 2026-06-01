using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Repositories.Interfaces
{
    public interface IProductoOrdenRepository : IRepository<ProductoOrden>
    {
        Task<IEnumerable<ProductoOrden>> ObtenerPorOrdenIdAsync(int ordenId);
        Task<bool> ExisteProductoEnOrdenAsync(int ordenId, int productoId);
    }
}
