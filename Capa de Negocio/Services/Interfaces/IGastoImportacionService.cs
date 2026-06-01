using Capa_de_Negocio.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Interfaces;
    public interface IGastoImportacionService
    {
        // Obtiene todos los gastos asociados a una orden de importacion especifica para el listado inicial.
        Task<IEnumerable<GastoImportacionDto>> ObtenerPorOrdenIdAsync(int ordenId);

        // Obtiene un gasto especifico por su ID, util para cargar los formularios de Edicion y Eliminacion
        Task<GastoImportacionDto?> ObtenerPorIdAsync(int id);

        // Registra un nuevo gasto ejecutando todas las validaciones de estado, duplicados, tasas y distribucion
        Task RegistrarAsync(GastoImportacionDto dto);

        // Edita un gasto existente validando que no se altere la orden original y reevaluando las reglas de negocio
        Task EditarAsync(GastoImportacionDto dto);

        // Elimina fisicamente un gasto siempre y cuando la orden de importacion se encuentre abierta
        Task EliminarAsync(int id);
    }
