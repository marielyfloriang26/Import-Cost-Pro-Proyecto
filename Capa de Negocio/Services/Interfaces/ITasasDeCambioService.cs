using Capa_de_Datos.Entities;
using Capa_de_Negocio.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Interfaces
{
    public interface ITasasDeCambioService
    {
        Task<IEnumerable<TasaCambioDto>> GetTasas();
        Task<IEnumerable<TasaCambioDto>> GetTasasActivas();
        Task<TasaCambioDto> GetTasaById(int id);
        Task<TasaCambioDto> CreateTasa(CrearTasaCambioDto tasa);
        Task<TasaCambioDto> UpdateTasa(int id, TasaCambioDto tasa);
        Task<bool> DeleteTasa(int id);
    }
}