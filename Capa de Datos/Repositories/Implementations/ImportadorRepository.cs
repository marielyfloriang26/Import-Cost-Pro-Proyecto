using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class ImportadorRepository : Repository<Importador>, IImportadorRepository
    {
        public ImportadorRepository(ImportCostContext context) : base(context) { }

        public async Task<IEnumerable<Importador>> ObtenerConPaisAsync()
        {
            // Usamos .Include() para hacer un inner join explicito con Países en SQL Server
            return await _context.Importadores
                                 .Include(i => i.Pais)
                                 .ToListAsync();
        }

        public async Task<bool> TieneOrdenesAsociadasAsync(int id)
        {
            // Validamos contra la colección de órdenes del sistema
            return await _context.OrdenesImportacion.AnyAsync(x => x.ImportadorId == id);
        }
    }
}
