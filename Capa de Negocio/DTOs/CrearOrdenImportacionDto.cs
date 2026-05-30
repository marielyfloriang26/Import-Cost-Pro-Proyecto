using Capa_de_Datos.Enums;
using System;

namespace Capa_de_Negocio.DTOs
{
    public class CrearOrdenImportacionDto
    {
        public string NumeroOrden { get; set; } = string.Empty;
        public int ImportadorId { get; set; }
        public int ProveedorId { get; set; }
        public int PaisOrigenId { get; set; }
        public int MonedaId { get; set; }
        public DateTime FechaOrden { get; set; }
        public MedioTransporte MedioTransporte { get; set; }
    }
}
