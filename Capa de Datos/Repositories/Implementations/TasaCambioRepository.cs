using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class TasaCambioRepository : Repository<TasaCambio>, ITasaCambioRepository
    {
        public TasaCambioRepository(ImportCostContext context) : base(context) { }
    }
}
