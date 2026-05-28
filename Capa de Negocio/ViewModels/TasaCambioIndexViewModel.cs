using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;

public class TasaCambioIndexViewModel
{
    public int IdTasaCambio { get; set; }

    [Display(Name = "Moneda Origen")]
    public string MonedaOrigenNombre { get; set; } = string.Empty;

    [Display(Name = "Moneda Destino")]
    public string MonedaDestinoNombre { get; set; } = string.Empty;

    [Display(Name = "Valor de la Tasa")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal ValorTasa { get; set; }

    [Display(Name = "Fecha de Vigencia")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime FechaVigencia { get; set; }

    [Display(Name = "Estado")]
    public bool Estado { get; set; }

    public string EstadoTexto => Estado ? "Activa" : "Inactiva";
}