namespace Capa_de_Datos.Entities
{
    public class CategoriaArancelaria
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PorcentajeArancel { get; set; }
        public bool AplicaItbis { get; set; }
        public bool AplicaSelectivo { get; set; }
        public decimal PorcentajeSelectivo { get; set; }
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
