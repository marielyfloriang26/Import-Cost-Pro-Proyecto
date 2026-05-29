using System.Collections.Generic;
using System.Threading.Tasks;
using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces
{
    public interface IImportadorService
    {
        Task<IEnumerable<ImportadorDto>> ObtenerTodosAsync();
        Task<ImportadorDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(ImportadorDto dto);
        Task EditarAsync(ImportadorDto dto);
        Task EliminarAsync(int id);
        
        // Sirve para los formularios de Órdenes de Importación
        Task<IEnumerable<ImportadorDto>> ObtenerActivosAsync();
    }
}