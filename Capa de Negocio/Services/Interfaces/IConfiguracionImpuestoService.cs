using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces;
    public interface IConfiguracionImpuestoService
    {
        Task<ConfiguracionImpuestoDto> ObtenerConfiguracionActualAsync();
        Task GuardarConfiguracionAsync(ConfiguracionImpuestoDto dto);
    }
