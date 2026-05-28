using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class PaisViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del país es requerido.")]
        [Display(Name = "Nombre del país")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código ISO es requerido.")]
        [Display(Name = "Código ISO")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "El código ISO debe tener entre 2 y 3 caracteres.")]
        public string CodigoIso { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Estado { get; set; } = true; // Por defecto Activo
    }
}