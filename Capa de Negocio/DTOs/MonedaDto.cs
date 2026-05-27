namespace Capa_de_Negocio.DTOs
{
    public class MonedaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string CodigoIso { get; set; } = null!;
        public string Simbolo { get; set; } = null!;
        public bool EsMonedaLocal { get; set; }
        public bool Estado { get; set; }
    }
}