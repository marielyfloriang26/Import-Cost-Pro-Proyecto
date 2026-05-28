using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class TasaCambioRepository : Repository<TasaCambio>, ITasaCambioRepository
    {
        public TasaCambioRepository(ImportCostContext context) : base(context) { }
        public async Task<bool> HasBeenUsedInLandedCostAsync(TasaCambio tasaAEditar)
        {
            var siguienteTasa = await _context.TasasCambio
                .Where(t => t.MonedaOrigenId == tasaAEditar.MonedaOrigenId &&
                            t.MonedaDestinoId == tasaAEditar.MonedaDestinoId &&
                            t.FechaVigencia.Date > tasaAEditar.FechaVigencia.Date &&
                            t.Estado == true)
                .OrderBy(t => t.FechaVigencia)
                .FirstOrDefaultAsync();

            DateTime inicioVigencia = tasaAEditar.FechaVigencia.Date;

            DateTime finVigencia = siguienteTasa != null
                ? siguienteTasa.FechaVigencia.Date.AddDays(-1)
                : DateTime.MaxValue;

            return await _context.LandedCostCalculos
                .AnyAsync(lc =>
                    lc.FechaCalculo.Date >= inicioVigencia &&
                    lc.FechaCalculo.Date <= finVigencia &&

                    lc.MonedaLocalUsadaId == tasaAEditar.MonedaDestinoId &&
                    lc.Orden != null && lc.Orden.MonedaId == tasaAEditar.MonedaOrigenId
                );
        }
    }
}
