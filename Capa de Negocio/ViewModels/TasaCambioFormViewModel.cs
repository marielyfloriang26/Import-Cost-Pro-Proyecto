using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;

public class TasaCambioFormViewModel
{
    public int IdTasaCambio { get; set; }

    [Required(ErrorMessage = "La moneda de origen es requerida.")]
    [Display(Name = "Moneda de Origen")]
    public int MonedaOrigenId { get; set; }

    [Required(ErrorMessage = "La moneda de destino es requerida.")]
    [Display(Name = "Moneda de Destino (Local)")]
    public int MonedaDestinoId { get; set; }

    [Required(ErrorMessage = "El valor de la tasa es requerido.")]
    [Range(0.000001, double.MaxValue, ErrorMessage = "El valor de la tasa debe ser mayor a cero.")]
    [Display(Name = "Valor de la Tasa")]
    public decimal ValorTasa { get; set; }

    [Required(ErrorMessage = "La fecha de vigencia es requerida.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Vigencia")]
    public DateTime FechaVigencia { get; set; } = DateTime.Now;

    [Display(Name = "Estado")]
    public bool Estado { get; set; } = true;
}