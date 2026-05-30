
using Capa_de_Datos.Enums;

namespace Capa_de_Negocio.DTOs 
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoReferencia { get; set; } = string.Empty;
        
        public int PaisOrigenId { get; set; }
        public string PaisOrigenNombre { get; set; } = string.Empty; // Para el listado
        
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty; // Para el listado

        public decimal PesoUnitario { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Ancho { get; set; }
        public decimal? Alto { get; set; }
        
        public UnidadMedida UnidadMedida { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }
}