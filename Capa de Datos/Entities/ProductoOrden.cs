namespace Capa_de_Datos.Entities
{
    public class ProductoOrden
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public virtual OrdenImportacion? Orden { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitarioFob { get; set; }
        public decimal MargenDeseado { get; set; }
    }
}
