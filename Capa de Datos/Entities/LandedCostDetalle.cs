namespace Capa_de_Datos.Entities
{
    public class LandedCostDetalle
    {
        public int Id { get; set; }
        public int CalculoId { get; set; }
        public virtual LandedCostCalculo? Calculo { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal FobOriginal { get; set; }
        public decimal FobLocal { get; set; }
        public decimal FleteAsignado { get; set; }
        public decimal SeguroAsignado { get; set; }
        public decimal Cif { get; set; }
        public decimal PorcentajeArancelUsado { get; set; }
        public decimal Arancel { get; set; }
        public decimal PorcentajeSelectivoUsado { get; set; }
        public decimal ImpuestoSelectivo { get; set; }
        public decimal TasaServicioAduanal { get; set; }
        public decimal Itbis { get; set; }
        public decimal GastosLocalesAsignados { get; set; }
        public decimal CostoTotalImportado { get; set; }
        public decimal CostoUnitarioImportado { get; set; }
        public decimal MargenDeseado { get; set; }
        public decimal PrecioVentaSugerido { get; set; }
    }
}
