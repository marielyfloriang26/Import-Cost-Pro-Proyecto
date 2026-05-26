namespace Capa_de_Datos.Entities
{
    public class TasaCambio
    {
        public int Id { get; set; }
        public int MonedaOrigenId { get; set; }
        public virtual Moneda? MonedaOrigen { get; set; }
        public int MonedaDestinoId { get; set; }
        public virtual Moneda? MonedaDestino { get; set; }
        public decimal ValorTasa { get; set; }
        public DateTime FechaVigencia { get; set; }
        public bool Estado { get; set; } = true;
    }
}
