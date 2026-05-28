using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class MonedaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la moneda es requerido.")]
        [Display(Name = "Nombre de la moneda")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código ISO es requerido.")]
        [Display(Name = "Código ISO")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código ISO debe tener exactamente 3 caracteres.")]
        public string CodigoIso { get; set; } = string.Empty;

        [Required(ErrorMessage = "El símbolo es requerido.")]
        [Display(Name = "Símbolo")]
        [StringLength(10, ErrorMessage = "El símbolo no puede exceder los 10 caracteres.")]
        public string Simbolo { get; set; } = string.Empty;

        [Display(Name = "Es moneda local")]
        public bool EsMonedaLocal { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Estado { get; set; } = true;
    }
}