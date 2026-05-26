namespace Capa_de_Datos.Entities
{
    public class Moneda
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoIso { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
        public bool EsMonedaLocal { get; set; }
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
        public virtual ICollection<TasaCambio> TasasOrigen { get; set; } = new List<TasaCambio>();
        public virtual ICollection<TasaCambio> TasasDestino { get; set; } = new List<TasaCambio>();
        public virtual ICollection<OrdenImportacion> OrdenesImportacion { get; set; } = new List<OrdenImportacion>();
        public virtual ICollection<GastoImportacion> GastosImportacion { get; set; } = new List<GastoImportacion>();
        public virtual ICollection<LandedCostCalculo> LandedCostCalculos { get; set; } = new List<LandedCostCalculo>();
    }
}
