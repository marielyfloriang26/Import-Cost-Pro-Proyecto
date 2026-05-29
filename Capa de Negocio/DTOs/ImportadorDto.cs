namespace Capa_de_Negocio.DTOs
{
    public class ImportadorDto
    {
        public int Id { get; set; }
        public string NombreRazonSocial { get; set; } = null!; 
        public string RncIdentificacion { get; set; } = null!; 
        public int PaisId { get; set; }
        public string? NombrePais { get; set; } 
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public bool Estado { get; set; }
    }
}