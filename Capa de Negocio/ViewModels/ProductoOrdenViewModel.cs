using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class ProductoOrdenViewModel
    {
        public int Id { get; set; }
        public int OrdenImportacionId { get; set; }

        [Required(ErrorMessage = "El producto es requerido.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string? NombreProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        [Display(Name = "Cantidad")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario FOB es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario FOB debe ser mayor que 0.")]
        [Display(Name = "Precio unitario FOB")]
        public decimal PrecioUnitarioFob { get; set; }

        [Required(ErrorMessage = "El margen de ganancia es requerido.")]
        [Range(0.0, 99.99, ErrorMessage = "El margen debe estar entre 0 y menor que 100.")]
        [Display(Name = "Margen de ganancia deseado (%)")]
        public decimal MargenGananciaDeseado { get; set; }
    }
}