using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;
    public class CategoriaArancelariaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código arancelario es requerido.")]
        [StringLength(20, ErrorMessage = "El código arancelario no puede exceder los 20 caracteres.")]
        [Display(Name = "Código arancelario")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre o descripción es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre o descripción no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre o descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El porcentaje de arancel es requerido.")]
        [Range(0.00, 100.00, ErrorMessage = "El porcentaje de arancel debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de arancel")]
        public decimal PorcentajeArancel { get; set; }

        [Required]
        [Display(Name = "Aplica ITBIS")]
        public bool AplicaItbis { get; set; }

        [Required]
        [Display(Name = "Aplica impuesto selectivo")]
        public bool AplicaSelectivo { get; set; }

        [Range(0.00, 100.00, ErrorMessage = "El porcentaje de impuesto selectivo debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de impuesto selectivo")]
        public decimal PorcentajeSelectivo { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true; // Por defecto activo al crear

        // Propiedad de control para bloquear inputs criticos en la vista de edicion
        public bool TieneProductosAsociados { get; set; }
    }
