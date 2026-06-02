using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class ProveedorRepository : Repository<Proveedor>, IProveedorRepository
    {
        public ProveedorRepository(ImportCostContext context) : base(context) { }

        public override async Task<Proveedor?> GetByIdAsync(int id)
        {
            return await _context.Proveedores
                .Include(p => p.Pais)
                .Include(p => p.MonedaPrincipal)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            return await _context.Proveedores
                .Include(p => p.Pais)
                .Include(p => p.MonedaPrincipal)
                .ToListAsync();
        }
    }
}
