using Capa_de_Negocio.DTOs;

namespace ImportCostPro.Capa_de_Negocio.ViewModels;

    public class ProveedorListadoViewModel
    {
        // Esta lista guardará los proveedores que traeremos del servicio para dibujarlos en la tabla
        public IEnumerable<ProveedorDTO> Proveedores { get; set; } = new List<ProveedorDTO>();
    }
