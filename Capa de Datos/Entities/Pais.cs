namespace Capa_de_Datos.Entities
{
    public class Pais
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoIso { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<Importador> Importadores { get; set; } = new List<Importador>();
        public virtual ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<OrdenImportacion> OrdenesImportacion { get; set; } = new List<OrdenImportacion>();
    }
}
