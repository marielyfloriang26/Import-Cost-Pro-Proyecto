using Capa_de_Datos.Enums;

namespace Capa_de_Datos.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoReferencia { get; set; } = string.Empty;
        public int PaisOrigenId { get; set; }
        public virtual Pais? PaisOrigen { get; set; }
        public int CategoriaId { get; set; }
        public virtual CategoriaArancelaria? Categoria { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Ancho { get; set; }
        public decimal? Alto { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<ProductoOrden> ProductosOrden { get; set; } = new List<ProductoOrden>();
        public virtual ICollection<LandedCostDetalle> LandedCostDetalles { get; set; } = new List<LandedCostDetalle>();
    }
}
