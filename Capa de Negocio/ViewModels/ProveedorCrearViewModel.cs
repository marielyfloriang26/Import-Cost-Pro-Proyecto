using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;

    public class ProveedorCrearViewModel
    {
        public int Id { get; set; } //para cuando toque editar

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre / Razón Social")]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Correo { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono de Contacto")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un país de origen.")]
        [Display(Name = "País de Procedencia")]
        public int PaisId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la moneda base.")]
        [Display(Name = "Moneda de Facturación")]
        public int MonedaPrincipalId { get; set; }

        public bool Estado { get; set; } = true; // Por defecto activo al crear

        // Listas que cargaran los paises y monedas
        public List<KeyValuePair<string, string>>? Paises { get; set; }
        public List<KeyValuePair<string, string>>? Monedas { get; set; }
    }
