using System.ComponentModel.DataAnnotations;

namespace Capa_de_Negocio.ViewModels;
    public class ConfiguracionImpuestoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El porcentaje general de ITBIS es requerido.")]
        [Range(0.00, 100.00, ErrorMessage = "El porcentaje general de ITBIS debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje General de ITBIS (%)")]
        public decimal? PorcentajeItbis { get; set; }

        [Required(ErrorMessage = "El porcentaje de tasa de servicio aduanal es requerido.")]
        [Range(0.00, 100.00, ErrorMessage = "El porcentaje de tasa de servicio aduanal debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de Tasa de Servicio Aduanal (%)")]
        public decimal? PorcentajeTasaAduanal { get; set; }
    }
