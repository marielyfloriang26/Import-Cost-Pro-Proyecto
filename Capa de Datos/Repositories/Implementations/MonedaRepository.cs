using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class MonedaRepository : Repository<Moneda>, IMonedaRepository
    {
        public MonedaRepository(ImportCostContext context) : base(context) { }

        public async Task<bool> RelacionesActivasAsync(int id)
        {
            // Validamos contra todas las colecciones que me mostraste en tu entidad Moneda
            return await _context.Proveedores.AnyAsync(x => x.MonedaPrincipalId == id) ||
                   await _context.TasasCambio.AnyAsync(x => x.MonedaOrigenId == id || x.MonedaDestinoId == id) ||
                   await _context.OrdenesImportacion.AnyAsync(x => x.MonedaId == id) ||
                   await _context.GastosImportacion.AnyAsync(x => x.MonedaId == id) ||
                   await _context.LandedCostCalculos.AnyAsync(x => x.MonedaLocalUsadaId == id);
        }
    }
}