using Capa_de_Datos.Enums;
using System;

namespace Capa_de_Negocio.DTOs;
    public class GastoImportacionDto
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public string? OrdenCodigo { get; set; } 
        public TipoGasto TipoGasto { get; set; }
        public decimal Monto { get; set; }
        public int MonedaId { get; set; }
        public string? MonedaNombre { get; set; }
        public string? MonedaSimbolo { get; set; }
        public MetodoDistribucion MetodoDistribucion { get; set; }
        public DateTime FechaGasto { get; set; }
    }
