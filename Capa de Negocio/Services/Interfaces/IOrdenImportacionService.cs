using Capa_de_Negocio.DTOs;
using Capa_de_Datos.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Interfaces
{
    public interface IOrdenImportacionService
    {
        Task<IEnumerable<OrdenImportacionDto>> ObtenerTodasAsync();
        Task<OrdenImportacionDto?> ObtenerPorIdAsync(int id);
        Task<OrdenImportacionDto> CrearAsync(CrearOrdenImportacionDto dto);
        Task<OrdenImportacionDto> EditarAsync(int id, OrdenImportacionDto dto);
        Task EliminarAsync(int id);
        Task<bool> CambiarEstadoAsync(int id, EstadoOrden nuevoEstado);
    }
}
