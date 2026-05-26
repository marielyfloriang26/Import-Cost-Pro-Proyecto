namespace Capa_de_Datos.Entities
{
    public class LandedCostCalculo
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public virtual OrdenImportacion? Orden { get; set; }
        public DateTime FechaCalculo { get; set; }
        public int MonedaLocalUsadaId { get; set; }
        public virtual Moneda? MonedaLocalUsada { get; set; }
        public decimal TasaCambioUsada { get; set; }
        public decimal PorcentajeItbisGeneral { get; set; }
        public decimal PorcentajeTasaAduanal { get; set; }
        public decimal FobTotalOriginal { get; set; }
        public decimal FobTotalLocal { get; set; }
        public decimal FleteTotalLocal { get; set; }
        public decimal SeguroTotalLocal { get; set; }
        public decimal CifTotal { get; set; }
        public decimal TotalArancel { get; set; }
        public decimal TotalImpuestoSelectivo { get; set; }
        public decimal TotalTasaServicio { get; set; }
        public decimal TotalItbis { get; set; }
        public decimal TotalGastosLocales { get; set; }
        public decimal CostoTotalImportacion { get; set; }
        public decimal CantidadTotalImportada { get; set; }

        // Relaciones
        public virtual ICollection<LandedCostDetalle> Detalles { get; set; } = new List<LandedCostDetalle>();
    }
}
