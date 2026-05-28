using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Repositories.Interfaces
{
    public interface IOrdenImportacionRepository : IRepository<OrdenImportacion>
    {
        Task<OrdenImportacion?> GetWithDetailsAsync(int id);
        Task<IEnumerable<OrdenImportacion>> GetAllWithDetailsAsync();
    }
}
