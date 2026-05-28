using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;

namespace Capa_de_Negocio.Servicios
{
    public class PaisService : IPaisService
    {
        private readonly IPaisRepository _paisRepository;
        private readonly ImportCostContext _context;

        // Inyectamos la interfaz del repositorio y el contexto de base de datos
        public PaisService(IPaisRepository paisRepository, ImportCostContext context)
        {
            _paisRepository = paisRepository;
            _context = context;
        }

        public async Task<IEnumerable<PaisDto>> ObtenerTodosAsync()
        {
            var paises = await _paisRepository.GetAllAsync();
            return paises.Select(p => new PaisDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoIso = p.CodigoIso,
                Estado = p.Estado
            });
        }

        public async Task<PaisDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _paisRepository.GetByIdAsync(id);
            if (p == null) return null;

            return new PaisDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoIso = p.CodigoIso,
                Estado = p.Estado
            };
        }

        public async Task CrearAsync(PaisDto paisDto)
        {
            if (string.IsNullOrWhiteSpace(paisDto.Nombre))
                throw new BusinessException("El nombre del país es requerido.");

            if (string.IsNullOrWhiteSpace(paisDto.CodigoIso))
                throw new BusinessException("El código ISO es requerido.");

            // Guardar en mayúsculas y limpiar espacios
            string isoFormateado = paisDto.CodigoIso.Trim().ToUpper();

            if (isoFormateado.Length < 2 || isoFormateado.Length > 3)
                throw new BusinessException("El código ISO debe tener entre 2 y 3 caracteres.");

            // No permitir duplicados. Usamos FindAsync del repositorio genérico
            var existente = await _paisRepository.FindAsync(p => p.CodigoIso == isoFormateado);
            if (existente.Any())
                throw new BusinessException("Ya existe un país registrado con este código ISO.");

            var nuevoPais = new Pais
            {
                Nombre = paisDto.Nombre.Trim(),
                CodigoIso = isoFormateado,
                Estado = paisDto.Estado
            };

            await _paisRepository.AddAsync(nuevoPais);
            await _context.SaveChangesAsync();
        }

        public async Task EditarAsync(PaisDto paisDto)
        {
            var paisExistente = await _paisRepository.GetByIdAsync(paisDto.Id);
            if (paisExistente == null)
                throw new BusinessException("El país a editar no existe.");

            if (string.IsNullOrWhiteSpace(paisDto.Nombre))
                throw new BusinessException("El nombre del país es requerido.");

            if (string.IsNullOrWhiteSpace(paisDto.CodigoIso))
                throw new BusinessException("El código ISO es requerido.");

            string isoFormateado = paisDto.CodigoIso.Trim().ToUpper();

            if (isoFormateado.Length < 2 || isoFormateado.Length > 3)
                throw new BusinessException("El código ISO debe tener entre 2 y 3 caracteres.");

            // Validar que no pertenezca a OTRO país
            var duplicado = await _paisRepository.FindAsync(p => p.CodigoIso == isoFormateado && p.Id != paisDto.Id);
            if (duplicado.Any())
                throw new BusinessException("Ya existe un país registrado con este código ISO.");

            paisExistente.Nombre = paisDto.Nombre.Trim();
            paisExistente.CodigoIso = isoFormateado;
            paisExistente.Estado = paisDto.Estado;

            _paisRepository.Update(paisExistente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var pais = await _paisRepository.GetByIdAsync(id);
            if (pais == null) return;

            // AQUÍ USA EL MÉTODO DE TU INTERFAZ DE DATOS (Por eso no se borra)
            bool estaEnUso = await _paisRepository.RelacionesActivasAsync(id);
            if (estaEnUso)
            {
                throw new BusinessException("No se puede eliminar este país porque está asociado a otros registros del sistema.");
            }

            _paisRepository.Remove(pais);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaisDto>> ObtenerActivosAsync()
        {
            // Usa el FindAsync que heredaste del repositorio genérico de tu equipo
            var paisesActivos = await _paisRepository.FindAsync(p => p.Estado == true);
            
            return paisesActivos.Select(p => new PaisDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CodigoIso = p.CodigoIso,
                Estado = p.Estado
            });
        }
    }
}