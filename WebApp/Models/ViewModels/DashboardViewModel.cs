using System.Collections.Generic;

namespace WebApp.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalOrdenes { get; set; }
    public int OrdenesAbiertas { get; set; }
    public int OrdenesCalculadas { get; set; }
    public int OrdenesCerradas { get; set; }
    public decimal TotalFobAcumulado { get; set; }
    
    public int TotalProductos { get; set; }
    public int TotalProveedores { get; set; }
    public int TotalImportadores { get; set; }

    public List<Capa_de_Negocio.ViewModels.OrdenImportacionIndexViewModel> Recientes { get; set; } = new();
}
