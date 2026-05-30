using System;
using System.ComponentModel.DataAnnotations;
using Capa_de_Datos.Enums;

namespace Capa_de_Negocio.ViewModels;

public class OrdenImportacionFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de orden es requerido.")]
    [StringLength(30, ErrorMessage = "El número de orden no puede exceder los 30 caracteres.")]
    [Display(Name = "Número de Orden")]
    public string NumeroOrden { get; set; } = string.Empty;

    [Required(ErrorMessage = "El importador es requerido.")]
    [Display(Name = "Importador")]
    public int ImportadorId { get; set; }

    [Required(ErrorMessage = "El proveedor es requerido.")]
    [Display(Name = "Proveedor")]
    public int ProveedorId { get; set; }

    [Required(ErrorMessage = "El país de origen es requerido.")]
    [Display(Name = "País de Origen")]
    public int PaisOrigenId { get; set; }

    [Required(ErrorMessage = "La moneda es requerida.")]
    [Display(Name = "Moneda")]
    public int MonedaId { get; set; }

    [Required(ErrorMessage = "La fecha de la orden es requerida.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de la Orden")]
    public DateTime FechaOrden { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La modalidad de transporte es requerida.")]
    [Display(Name = "Modalidad de Transporte")]
    public MedioTransporte MedioTransporte { get; set; }

    [Display(Name = "Estado")]
    public EstadoOrden EstadoOrden { get; set; } = EstadoOrden.Abierta;
}
