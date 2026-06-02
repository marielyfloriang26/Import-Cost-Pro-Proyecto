using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public ProductoRepository(ImportCostContext context) : base(context) { }

        public override async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.PaisOrigen)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.PaisOrigen)
                .Include(p => p.Categoria)
                .ToListAsync();
        }
    }
}
