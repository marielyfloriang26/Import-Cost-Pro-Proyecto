using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class MonedaRepository : Repository<Moneda>, IMonedaRepository
    {
        public MonedaRepository(ImportCostContext context) : base(context) { }
    }
}
