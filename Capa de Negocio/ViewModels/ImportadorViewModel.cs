using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class ImportadorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre o razón social es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre o razón social no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre o razón social")]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RNC o identificación fiscal es requerido.")]
        [StringLength(20, ErrorMessage = "El RNC no puede exceder los 20 caracteres.")]
        [Display(Name = "RNC o identificación fiscal")]
        public string RncIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido.")]
        [Display(Name = "País")]
        public int PaisId { get; set; }

        [Display(Name = "País")]
        public string? NombrePais { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string? Correo { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true; // Por defecto activo tal como pide el PDF
    }
}