using Capa_de_Datos.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;
    public class GastoImportacionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La orden de importación es requerida.")]
        public int OrdenId { get; set; }
        
        public string? OrdenCodigo { get; set; } // Deshabilitado/Lectura en pantalla

        [Required(ErrorMessage = "El tipo de gasto es requerido.")]
        public TipoGasto TipoGasto { get; set; }

        [Required(ErrorMessage = "El monto es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La moneda es requerida.")]
        public int MonedaId { get; set; }
        
        public string? MonedaNombre { get; set; }

        [Required(ErrorMessage = "El método de distribución es requerido.")]
        public MetodoDistribucion MetodoDistribucion { get; set; }

        [Required(ErrorMessage = "La fecha es requerida.")]
        [DataType(DataType.Date)]
        public DateTime FechaGasto { get; set; } = DateTime.Now;
    }
