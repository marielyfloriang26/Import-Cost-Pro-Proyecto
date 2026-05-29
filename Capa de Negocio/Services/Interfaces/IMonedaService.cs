using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces
{
    public interface IMonedaService
    {
        Task<IEnumerable<MonedaDto>> ObtenerTodasAsync();
        Task<MonedaDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(MonedaDto monedaDto);
        Task EditarAsync(MonedaDto monedaDto);
        Task EliminarAsync(int id);
        
        // Para llenar los dropdowns en Proveedores, Órdenes, etc.
        Task<IEnumerable<MonedaDto>> ObtenerActivasAsync();
    }
}