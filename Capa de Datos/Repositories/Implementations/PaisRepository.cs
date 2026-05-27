using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class PaisRepository : Repository<Pais>, IPaisRepository
    {
        public PaisRepository(ImportCostContext context) : base(context) { }

        public async Task<bool> RelacionesActivasAsync(int id)
        {
            // Validamos eficientemente con AnyAsync en cada DbSet relacionado
            return await _context.Proveedores.AnyAsync(x => x.PaisId == id) ||
                   await _context.Importadores.AnyAsync(x => x.PaisId == id) ||
                   await _context.Productos.AnyAsync(x => x.PaisOrigenId == id) ||
                   await _context.OrdenesImportacion.AnyAsync(x => x.PaisOrigenId == id);
        }
    }
}
