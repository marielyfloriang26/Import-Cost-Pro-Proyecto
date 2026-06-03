using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;

namespace Capa_de_Negocio.Servicios
{
    public class ImportadorService : IImportadorService
    {
        private readonly IImportadorRepository _importadorRepository;
        private readonly IPaisRepository _paisRepository;

        public ImportadorService(IImportadorRepository importadorRepository, IPaisRepository paisRepository)
        {
            _importadorRepository = importadorRepository;
            _paisRepository = paisRepository;
        }

        public async Task<IEnumerable<ImportadorDto>> ObtenerTodosAsync()
        {
            var importadores = await _importadorRepository.ObtenerConPaisAsync();
            return importadores.Select(i => new ImportadorDto
            {
                Id = i.Id,
                NombreRazonSocial = i.NombreRazonSocial,
                RncIdentificacion = i.RncIdentificacion,
                PaisId = i.PaisId,
                NombrePais = i.Pais?.Nombre ?? "No asignado",
                Telefono = i.Telefono,
                Correo = i.Correo,
                Direccion = i.Direccion,
                Estado = i.Estado
            });
        }

        public async Task<ImportadorDto?> ObtenerPorIdAsync(int id)
        {
            var i = await _importadorRepository.GetByIdAsync(id);
            if (i == null) return null;

            return new ImportadorDto
            {
                Id = i.Id,
                NombreRazonSocial = i.NombreRazonSocial,
                RncIdentificacion = i.RncIdentificacion,
                PaisId = i.PaisId,
                Telefono = i.Telefono,
                Correo = i.Correo,
                Direccion = i.Direccion,
                Estado = i.Estado
            };
        }

        public async Task<IEnumerable<ImportadorDto>> ObtenerActivosAsync()
        {
            var activos = await _importadorRepository.FindAsync(i => i.Estado == true);
            return activos.Select(i => new ImportadorDto
            {
                Id = i.Id,
                NombreRazonSocial = i.NombreRazonSocial,
                RncIdentificacion = i.RncIdentificacion,
                PaisId = i.PaisId,
                Estado = i.Estado
            });
        }

        public async Task CrearAsync(ImportadorDto dto)
        {
            ValidarCamposEstructurales(dto);
            
            //Ignorar espacios al inicio y al final en el RNC
            string rncFormateado = dto.RncIdentificacion.Trim();

            // El RNC no puede repetirse en el sistema
            var existeRnc = await _importadorRepository.FindAsync(x => x.RncIdentificacion == rncFormateado);
            if (existeRnc.Any())
                throw new ConflictException("Ya existe un importador registrado con este RNC o identificación fiscal.");

            // Validar que el país exista y esté activo en su propio mantenimiento
            var paisSeleccionado = await _paisRepository.GetByIdAsync(dto.PaisId);
            if (paisSeleccionado == null)
                throw new NotFoundException("El país seleccionado no es válido.");
            
            if (!paisSeleccionado.Estado)
                throw new ValidationException("El país seleccionado debe estar activo.");

            var nuevoImportador = new Importador
            {
                NombreRazonSocial = dto.NombreRazonSocial.Trim(),
                RncIdentificacion = rncFormateado,
                PaisId = dto.PaisId,
                Telefono = dto.Telefono?.Trim(),
                Correo = dto.Correo?.Trim(),
                Direccion = dto.Direccion?.Trim(),
                Estado = dto.Estado
            };

            await _importadorRepository.AddAsync(nuevoImportador);
            await _importadorRepository.SaveAsync();
        }

        public async Task EditarAsync(ImportadorDto dto)
        {
            var importadorExistente = await _importadorRepository.GetByIdAsync(dto.Id);
            if (importadorExistente == null)
                throw new NotFoundException("El importador a editar no existe.");

            ValidarCamposEstructurales(dto);
            string rncFormateado = dto.RncIdentificacion.Trim();

            // Validar que el RNC no pertenezca a OTRO importador diferente
            var duplicadoRnc = await _importadorRepository.FindAsync(x => x.RncIdentificacion == rncFormateado && x.Id != dto.Id);
            if (duplicadoRnc.Any())
                throw new ConflictException("Ya existe un importador registrado con este RNC o identificación fiscal.");

            // Si intenta modificar el RNC, verificar si ya tiene órdenes asociadas para bloquearlo
            if (importadorExistente.RncIdentificacion != rncFormateado)
            {
                bool tieneOrdenes = await _importadorRepository.TieneOrdenesAsociadasAsync(dto.Id);
                if (tieneOrdenes)
                {
                    throw new ValidationException("No se puede modificar el RNC o identificación fiscal de este importador porque ya tiene órdenes de importación registradas.");
                }
            }

            var paisSeleccionado = await _paisRepository.GetByIdAsync(dto.PaisId);
            if (paisSeleccionado == null)
                throw new NotFoundException("El país seleccionado no es válido.");

            // Actualizamos propiedades físicas
            importadorExistente.NombreRazonSocial = dto.NombreRazonSocial.Trim();
            importadorExistente.RncIdentificacion = rncFormateado;
            importadorExistente.PaisId = dto.PaisId;
            importadorExistente.Telefono = dto.Telefono?.Trim();
            importadorExistente.Correo = dto.Correo?.Trim();
            importadorExistente.Direccion = dto.Direccion?.Trim();
            importadorExistente.Estado = dto.Estado;

            _importadorRepository.Update(importadorExistente);
            await _importadorRepository.SaveAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var importador = await _importadorRepository.GetByIdAsync(id);
            if (importador == null) return;

            // No se puede eliminar si posee órdenes históricas asociadas
            bool tieneOrdenes = await _importadorRepository.TieneOrdenesAsociadasAsync(id);
            if (tieneOrdenes)
            {
                throw new ValidationException("No se puede eliminar este importador porque tiene órdenes de importación registradas.");
            }

            _importadorRepository.Remove(importador);
            await _importadorRepository.SaveAsync();
        }

        private void ValidarCamposEstructurales(ImportadorDto dto)
        {
            // Centralizamos las validaciones de longitud y obligatoriedad en el negocio
            if (string.IsNullOrWhiteSpace(dto.NombreRazonSocial))
                throw new ValidationException("El nombre o razón social es requerido.");

            if (dto.NombreRazonSocial.Length > 150)
                throw new ValidationException("El nombre o razón social debe tener un máximo de 150 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.RncIdentificacion))
                throw new ValidationException("El RNC o identificación fiscal es requerido.");

            if (dto.RncIdentificacion.Length > 20)
                throw new ValidationException("El RNC o identificación fiscal debe tener un máximo de 20 caracteres.");

            if (!string.IsNullOrWhiteSpace(dto.Telefono) && dto.Telefono.Length > 20)
                throw new ValidationException("El teléfono debe tener un máximo de 20 caracteres.");

            if (!string.IsNullOrWhiteSpace(dto.Direccion) && dto.Direccion.Length > 250)
                throw new ValidationException("La dirección debe tener un máximo de 250 caracteres.");

            if (!string.IsNullOrWhiteSpace(dto.Correo) && dto.Correo.Length > 100)
                throw new ValidationException("El correo electrónico debe tener un máximo de 100 caracteres.");
        }
    }
}