using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Negocio.DTOs
{
    public class CrearTasaCambioDto
    {
        public int MonedaOrigenId { get; set; }

        public int MonedaDestinoId { get; set; }

        public decimal ValorTasa { get; set; }

        public DateTime FechaVigencia { get; set; }

        public bool Estado { get; set; }
    }
}
