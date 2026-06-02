using System;
using System.ComponentModel.DataAnnotations;
using Capa_de_Datos.Enums;

namespace Capa_de_Negocio.ViewModels;

public class OrdenImportacionIndexViewModel
{
    public int Id { get; set; }

    [Display(Name = "Número de Orden")]
    public string NumeroOrden { get; set; } = string.Empty;

    [Display(Name = "Importador")]
    public string ImportadorNombre { get; set; } = string.Empty;

    [Display(Name = "Proveedor")]
    public string ProveedorNombre { get; set; } = string.Empty;

    [Display(Name = "País de Origen")]
    public string PaisOrigenNombre { get; set; } = string.Empty;

    [Display(Name = "Moneda")]
    public string MonedaNombre { get; set; } = string.Empty;

    [Display(Name = "Fecha")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime FechaOrden { get; set; }

    [Display(Name = "Fecha de Cierre")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime? FechaCierre { get; set; }

    [Display(Name = "Transporte")]
    public MedioTransporte MedioTransporte { get; set; }

    [Display(Name = "Estado")]
    public EstadoOrden EstadoOrden { get; set; }

    [Display(Name = "FOB Total")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal FobTotal { get; set; }

    [Display(Name = "Total Estimado")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal TotalEstimadoLandedCost { get; set; }
}
