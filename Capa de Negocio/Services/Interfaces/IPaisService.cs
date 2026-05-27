using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces
{
    public interface IPaisService
    {
        Task<IEnumerable<PaisDto>> ObtenerTodosAsync();
        Task<PaisDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(PaisDto paisDto);
        Task EditarAsync(PaisDto paisDto);
        Task EliminarAsync(int id);
        Task<IEnumerable<PaisDto>> ObtenerActivosAsync();
    }
}