using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Capa_de_Negocio.DTOs;

namespace WebApp.Models.ViewModels
{
    public class LandedCostIndexViewModel
    {
        public int? SelectedOrdenId { get; set; }
        public SelectList? OrdenesAbiertas { get; set; }
    }

    public class LandedCostCalculoViewModel
    {
        public LandedCostCalculoDto? Calculo { get; set; }
        public bool EsCalculoOficial { get; set; }
    }
}
