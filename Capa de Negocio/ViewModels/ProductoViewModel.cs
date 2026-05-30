using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;
    public class ProductoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre del producto debe tener un máximo de 150 caracteres.")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código o referencia es requerido.")]
        [StringLength(50, ErrorMessage = "El código o referencia debe tener un máximo de 50 caracteres.")]
        [Display(Name = "Código / Referencia (SKU)")]
        public string CodigoReferencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país de origen predeterminado es requerido.")]
        [Display(Name = "País de Origen Predeterminado")]
        public int PaisOrigenId { get; set; }

        [Required(ErrorMessage = "La categoría arancelaria es requerida.")]
        [Display(Name = "Categoría Arancelaria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El peso unitario es requerido.")]
        [Range(0.001, double.MaxValue, ErrorMessage = "El peso unitario debe ser mayor que 0.")]
        [Display(Name = "Peso Unitario (Kg)")]
        public decimal PesoUnitario { get; set; }

        // Campos de dimensiones
        [Range(0.001, double.MaxValue, ErrorMessage = "El largo debe ser mayor que 0.")]
        [Display(Name = "Largo (cm)")]
        public decimal? Largo { get; set; }

        [Range(0.001, double.MaxValue, ErrorMessage = "El ancho debe ser mayor que 0.")]
        [Display(Name = "Ancho (cm)")]
        public decimal? Ancho { get; set; }

        [Range(0.001, double.MaxValue, ErrorMessage = "El alto debe ser mayor que 0.")]
        [Display(Name = "Alto (cm)")]
        public decimal? Alto { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        [Display(Name = "Unidad de Medida")]
        public string UnidadMedidaSelected { get; set; } = "Unidad"; // se maneja como texto en la vista y se convierte a Enum en el controlador

        [StringLength(250, ErrorMessage = "La descripción debe tener un máximo de 250 caracteres.")]
        [Display(Name = "Descripción Adicional")]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true; // Por defecto activo al crear

        // Propiedades para mostrar texto legible en la tabla del Index
        public string? PaisOrigenNombre { get; set; }
        public string? CategoriaNombre { get; set; }

        // Listas que usara el comando asp-items para cargar los combobox en el HTML
        public List<KeyValuePair<string, string>>? PaisesOptions { get; set; }
        public List<KeyValuePair<string, string>>? CategoriasOptions { get; set; }
        public List<string> UnidadesMedidaOptions { get; set; } = Enum.GetNames(typeof(Capa_de_Datos.Enums.UnidadMedida)).ToList();
    }
