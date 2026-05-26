namespace Capa_de_Datos.Entities
{
    public class Importador
    {
        public int Id { get; set; }
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string RncIdentificacion { get; set; } = string.Empty;
        public int PaisId { get; set; }
        public virtual Pais? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<OrdenImportacion> OrdenesImportacion { get; set; } = new List<OrdenImportacion>();
    }
}
