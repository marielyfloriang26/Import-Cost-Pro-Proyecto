using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class OrdenImportacionRepository : Repository<OrdenImportacion>, IOrdenImportacionRepository
    {
        public OrdenImportacionRepository(ImportCostContext context) : base(context) { }

        public async Task<OrdenImportacion?> GetWithDetailsAsync(int id)
        {
            return await _context.OrdenesImportacion
                .Include(o => o.Importador)
                .Include(o => o.Proveedor)
                .Include(o => o.PaisOrigen)
                .Include(o => o.Moneda)
                .Include(o => o.ProductosOrden)
                .Include(o => o.LandedCostCalculo)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<OrdenImportacion>> GetAllWithDetailsAsync()
        {
            return await _context.OrdenesImportacion
                .Include(o => o.Importador)
                .Include(o => o.Proveedor)
                .Include(o => o.PaisOrigen)
                .Include(o => o.Moneda)
                .Include(o => o.ProductosOrden)
                .Include(o => o.LandedCostCalculo)
                .ToListAsync();
        }
    }
}
