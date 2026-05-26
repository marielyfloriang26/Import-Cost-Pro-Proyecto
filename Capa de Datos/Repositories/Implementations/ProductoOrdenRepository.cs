using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;

namespace Capa_de_Datos.Repositories.Implementations
{
    public class ProductoOrdenRepository : Repository<ProductoOrden>, IProductoOrdenRepository
    {
        public ProductoOrdenRepository(ImportCostContext context) : base(context) { }
    }
}
