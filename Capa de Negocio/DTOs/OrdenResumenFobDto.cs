using System;
using System.Collections.Generic;
using Capa_de_Datos.Enums;

namespace Capa_de_Negocio.DTOs
{
    public class OrdenResumenFobDto
    {
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; } = null!;
        public EstadoOrden EstadoOrden { get; set; }
        public string MonedaNombre { get; set; } = null!;
        public string ProveedorNombre { get; set; } = null!;

        // Totales calculados generales exigidos por el PDF
        public decimal CantidadTotalProductos { get; set; }
        public decimal FobTotalOrden { get; set; }
        public decimal PesoTotalOrden { get; set; }
        public decimal VolumenTotalOrden { get; set; }
        public bool TieneProductosSinDimensiones { get; set; }

        public List<ProductoOrdenDto> Productos { get; set; } = new List<ProductoOrdenDto>();
        public List<GastoImportacionDto> Gastos { get; set; } = new List<GastoImportacionDto>();
    }
}