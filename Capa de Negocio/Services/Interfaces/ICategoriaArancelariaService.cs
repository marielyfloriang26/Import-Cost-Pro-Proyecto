using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces
{
    public interface ICategoriaArancelariaService
    {
        // (Index), Muestra todas las categorias registradas
        Task<IEnumerable<CategoriaArancelariaDTO>> ObtenerTodasAsync();

        // Para el Dropdown de Productos, Solo debe retornar las categorias activas
        Task<IEnumerable<CategoriaArancelariaDTO>> ObtenerActivasAsync();

        // Para cargar los datos en los formularios de Ver Detalle, Editar o Confirmar Eliminacion
        Task<CategoriaArancelariaDTO?> ObtenerPorIdAsync(int id);

        Task<bool> CrearAsync(CategoriaArancelariaDTO dto);

        Task<bool> EditarAsync(CategoriaArancelariaDTO dto);

        Task<bool> EliminarAsync(int id);

        // Validacion para asegurar que el codigo arancelario sea único
        Task<bool> ExisteCodigoAsync(string codigo, int idExcluir = 0);
    }
}