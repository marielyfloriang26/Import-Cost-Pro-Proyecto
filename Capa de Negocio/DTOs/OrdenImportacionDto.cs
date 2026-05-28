using Capa_de_Datos.Enums;
using System;

namespace Capa_de_Negocio.DTOs
{
    public class OrdenImportacionDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        
        public int ImportadorId { get; set; }
        public string ImportadorNombre { get; set; } = string.Empty;

        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;

        public int PaisOrigenId { get; set; }
        public string PaisOrigenNombre { get; set; } = string.Empty;

        public int MonedaId { get; set; }
        public string MonedaNombre { get; set; } = string.Empty;

        public DateTime FechaOrden { get; set; }
        public MedioTransporte MedioTransporte { get; set; }
        public EstadoOrden EstadoOrden { get; set; }

        public decimal FobTotal { get; set; }
        public decimal TotalEstimadoLandedCost { get; set; }
    }
}
