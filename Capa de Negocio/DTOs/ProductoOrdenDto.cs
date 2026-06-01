using System;

namespace Capa_de_Negocio.DTOs
{
    public class ProductoOrdenDto
    {
        public int Id { get; set; }
        public int OrdenImportacionId { get; set; }
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; }
        public string? CodigoReferencia { get; set; }
        
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitarioFob { get; set; }
        public decimal MargenGananciaDeseado { get; set; }

        // Propiedades calculadas automáticas exigidas por el PDF
        public decimal FobTotal => Cantidad * PrecioUnitarioFob;
        public decimal PesoTotal { get; set; }
        public decimal? VolumenTotal { get; set; }
    }
}