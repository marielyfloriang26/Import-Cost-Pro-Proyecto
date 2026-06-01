using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class ProductoOrdenRepository : Repository<ProductoOrden>, IProductoOrdenRepository
    {
        public ProductoOrdenRepository(ImportCostContext context) : base(context) { }
        public async Task<IEnumerable<ProductoOrden>> ObtenerPorOrdenIdAsync(int ordenId)
        {
            // COMENTARIO TÉCNICO: Hacemos un .Include hacia Producto para traernos 
            // los campos físicos necesarios para calcular peso y volumen.
            return await _context.Set<ProductoOrden>()
                                 .Include(po => po.Producto)
                                 .Where(po => po.OrdenId == ordenId)
                                 .ToListAsync();
        }

        public async Task<bool> ExisteProductoEnOrdenAsync(int ordenId, int productoId)
        {
            return await _context.Set<ProductoOrden>()
                                 .AnyAsync(po => po.OrdenId == ordenId && po.ProductoId == productoId);
        }
    }
}
