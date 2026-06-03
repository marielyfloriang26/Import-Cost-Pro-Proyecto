using Capa_de_Datos.Enums;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Implementations
{
    public class ConfirmacionCierreService : IConfirmacionCierreService
    {
        private readonly IOrdenImportacionRepository _ordenRepository;

        public ConfirmacionCierreService(IOrdenImportacionRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }

        public async Task<bool> CerrarOrdenAsync(int ordenId)
        {
            var orden = await _ordenRepository.GetWithDetailsAsync(ordenId);

            if (orden == null)
            {
                throw new NotFoundException($"No se encontró la orden con ID {ordenId}.");
            }

            // Validaciones según el documento funcional
            if (orden.EstadoOrden == EstadoOrden.Cerrada)
            {
                throw new ValidationException("La orden ya se encuentra cerrada.");
            }

            if (orden.EstadoOrden == EstadoOrden.Cancelada)
            {
                throw new ValidationException("No se puede cerrar una orden que ha sido cancelada.");
            }

            if (orden.EstadoOrden != EstadoOrden.Calculada)
            {
                throw new ValidationException("Solo se pueden cerrar órdenes que estén en estado Calculada.");
            }

            if (orden.LandedCostCalculo == null)
            {
                throw new ValidationException("No se puede cerrar esta orden porque no tiene un cálculo oficial de landed cost guardado.");
            }

            if (orden.ProductosOrden == null || !orden.ProductosOrden.Any())
            {
                throw new ValidationException("La orden debe tener productos registrados para ser cerrada.");
            }

            if (orden.LandedCostCalculo.CostoTotalImportacion <= 0)
            {
                throw new ValidationException("El cálculo oficial debe tener un costo total de importación mayor que 0 para proceder con el cierre.");
            }

            // Cambio de estado
            orden.EstadoOrden = EstadoOrden.Cerrada;
            orden.FechaCierre = System.DateTime.Now;

            _ordenRepository.Update(orden);
            await _ordenRepository.SaveAsync();

            return true;
        }
    }
}
