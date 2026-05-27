using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Repositories.Interfaces
{
    public interface IMonedaRepository : IRepository<Moneda>
    {
        // Método especializado para validar si la moneda tiene dependencias activas
        Task<bool> RelacionesActivasAsync(int id);
    }
}
