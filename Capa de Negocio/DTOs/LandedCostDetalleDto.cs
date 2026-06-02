namespace Capa_de_Negocio.DTOs
{
    public class LandedCostDetalleDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
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
