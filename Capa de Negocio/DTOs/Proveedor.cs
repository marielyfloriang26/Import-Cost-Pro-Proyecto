namespace Capa_de_Negocio.DTOs
{
    public class ProveedorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PaisId { get; set; }
        public string PaisNombre { get; set; } = string.Empty; // Para mostrar el nombre del país en la tabla
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public int MonedaPrincipalId { get; set; }
        public string MonedaNombre { get; set; } = string.Empty; // Para mostrar el nombre de la moneda en la tabla
        public bool Estado { get; set; }
    }
}