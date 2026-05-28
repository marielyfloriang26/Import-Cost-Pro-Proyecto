using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Repositories.Interfaces
{
    public interface IPaisRepository : IRepository<Pais>
    {
        // Metodo para verificar si el país tiene dependencias activas
        Task<bool> RelacionesActivasAsync(int id);
    }
}
