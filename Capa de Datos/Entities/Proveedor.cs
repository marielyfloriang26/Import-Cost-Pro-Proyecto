namespace Capa_de_Datos.Entities
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PaisId { get; set; }
        public virtual Pais? Pais { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public int MonedaPrincipalId { get; set; }
        public virtual Moneda? MonedaPrincipal { get; set; }
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<OrdenImportacion> OrdenesImportacion { get; set; } = new List<OrdenImportacion>();
    }
}
