using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Interfaces;

namespace Capa_de_Negocio.Implementations;
    public class ConfiguracionImpuestoService : IConfiguracionImpuestoService
    {
        private readonly IConfiguracionImpuestoRepository _repository;

        public ConfiguracionImpuestoService(IConfiguracionImpuestoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConfiguracionImpuestoDto> ObtenerConfiguracionActualAsync()
        {
            var configs = await _repository.GetAllAsync();
            var actual = configs.FirstOrDefault();

            if (actual == null)
            {
                return new ConfiguracionImpuestoDto { Id = 0, PorcentajeItbis = 0, PorcentajeTasaAduanal = 0 };
            }

            return new ConfiguracionImpuestoDto
            {
                Id = actual.Id,
                PorcentajeItbis = actual.PorcentajeItbis,
                PorcentajeTasaAduanal = actual.PorcentajeTasaAduanal
            };
        }

        public async Task GuardarConfiguracionAsync(ConfiguracionImpuestoDto dto)
        {
            
            var configs = await _repository.GetAllAsync();
            var actual = configs.FirstOrDefault();

            if (actual == null)
            {
                // Crear nueva configuracion
                var nuevaConfig = new ConfiguracionImpuesto
                {
                    PorcentajeItbis = dto.PorcentajeItbis!.Value,
                    PorcentajeTasaAduanal = dto.PorcentajeTasaAduanal!.Value
                };
                await _repository.AddAsync(nuevaConfig); 
            }
            else
            {
                // Editar configuracion existente
                actual.PorcentajeItbis = dto.PorcentajeItbis!.Value;
                actual.PorcentajeTasaAduanal = dto.PorcentajeTasaAduanal!.Value;
                
                
                _repository.Update(actual); 
            }

            //  Guardar los cambios asincronamente en la base de datos
            await _repository.SaveAsync();
        }
    }
