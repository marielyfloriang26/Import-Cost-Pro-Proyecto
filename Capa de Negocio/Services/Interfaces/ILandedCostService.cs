using Capa_de_Negocio.DTOs;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Interfaces
{
    public interface ILandedCostService
    {
        Task<LandedCostCalculoDto> CalcularAsync(int ordenId);
        Task<LandedCostCalculoDto> GuardarCalculoOficialAsync(int ordenId);
        Task<LandedCostCalculoDto?> ObtenerPorOrdenIdAsync(int ordenId);
    }
}
