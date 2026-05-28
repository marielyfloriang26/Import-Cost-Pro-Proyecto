using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Negocio.DTOs
{
    public class TasaCambioDto
    {
        public int IdTasaCambio { get; set; }
        
        public int MonedaOrigenId { get; set; }
        public string MonedaOrigenNombre { get; set; } = string.Empty;

        public int MonedaDestinoId { get; set; }
        public string MonedaDestinoNombre { get; set; } = string.Empty;

        public decimal ValorTasa { get; set; }

        public DateTime FechaVigencia { get; set; }

        public bool Estado { get; set; }
    }
}
