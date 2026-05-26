using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class PaisRepository : Repository<Pais>, IPaisRepository
    {
        public PaisRepository(ImportCostContext context) : base(context) { }
    }
}
