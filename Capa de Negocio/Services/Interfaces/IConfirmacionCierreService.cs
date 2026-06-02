using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Interfaces
{
    public interface IConfirmacionCierreService
    {
        Task<bool> CerrarOrdenAsync(int ordenId);
    }
}
