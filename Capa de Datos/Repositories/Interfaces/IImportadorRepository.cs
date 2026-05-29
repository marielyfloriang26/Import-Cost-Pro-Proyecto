using Capa_de_Datos.Entities;

namespace Capa_de_Datos.Repositories.Interfaces
{
    public interface IImportadorRepository : IRepository<Importador>
    {
        // Verifica si el importador ya tiene ordenes relacionadas históricamente
        Task<bool> TieneOrdenesAsociadasAsync(int id);
        
        // Trae los importadores incluyendo la tabla de paises para ver el nombre del pais en el listado
        Task<IEnumerable<Importador>> ObtenerConPaisAsync();
    }
}
