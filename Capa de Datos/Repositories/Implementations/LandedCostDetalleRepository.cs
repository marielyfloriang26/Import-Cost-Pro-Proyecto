using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class LandedCostDetalleRepository : Repository<LandedCostDetalle>, ILandedCostDetalleRepository
    {
        public LandedCostDetalleRepository(ImportCostContext context) : base(context) { }
    }
}
