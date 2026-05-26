using Capa_de_Datos.Enums;

namespace Capa_de_Datos.Entities
{
    public class GastoImportacion
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public virtual OrdenImportacion? Orden { get; set; }
        public TipoGasto TipoGasto { get; set; }
        public decimal Monto { get; set; }
        public int MonedaId { get; set; }
        public virtual Moneda? Moneda { get; set; }
        public MetodoDistribucion MetodoDistribucion { get; set; }
        public DateTime FechaGasto { get; set; }
    }
}
