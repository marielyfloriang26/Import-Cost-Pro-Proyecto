using Capa_de_Datos.Enums;

namespace Capa_de_Datos.Entities
{
    public class OrdenImportacion
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public int ImportadorId { get; set; }
        public virtual Importador? Importador { get; set; }
        public int ProveedorId { get; set; }
        public virtual Proveedor? Proveedor { get; set; }
        public int PaisOrigenId { get; set; }
        public virtual Pais? PaisOrigen { get; set; }
        public int MonedaId { get; set; }
        public virtual Moneda? Moneda { get; set; }
        public DateTime FechaOrden { get; set; }
        public DateTime? FechaCierre { get; set; }
        public MedioTransporte MedioTransporte { get; set; }
        public EstadoOrden EstadoOrden { get; set; } = EstadoOrden.Abierta;

        // Relaciones
        public virtual ICollection<ProductoOrden> ProductosOrden { get; set; } = new List<ProductoOrden>();
        public virtual ICollection<GastoImportacion> GastosImportacion { get; set; } = new List<GastoImportacion>();
        public virtual LandedCostCalculo? LandedCostCalculo { get; set; }
    }
}
