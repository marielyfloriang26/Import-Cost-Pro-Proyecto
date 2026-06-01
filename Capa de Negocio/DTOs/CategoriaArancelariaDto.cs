namespace Capa_de_Negocio.DTOs;
    public class CategoriaArancelariaDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public decimal PorcentajeArancel { get; set; }

        public bool AplicaItbis { get; set; }

        public bool AplicaSelectivo { get; set; }

        public decimal PorcentajeSelectivo { get; set; }

        public bool Estado { get; set; }

        public bool TieneProductosAsociados { get; set; }
    }
