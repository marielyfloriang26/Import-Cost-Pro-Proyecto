using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;

namespace Capa_de_Negocio.Servicios
{
    public class MonedaService : IMonedaService
    {
        private readonly IMonedaRepository _monedaRepository;

        public MonedaService(IMonedaRepository monedaRepository)
        {
            _monedaRepository = monedaRepository;
        }

        public async Task<IEnumerable<MonedaDto>> ObtenerTodasAsync()
        {
            var monedas = await _monedaRepository.GetAllAsync();
            return monedas.Select(m => new MonedaDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                CodigoIso = m.CodigoIso,
                Simbolo = m.Simbolo,
                EsMonedaLocal = m.EsMonedaLocal,
                Estado = m.Estado
            });
        }

        public async Task<MonedaDto?> ObtenerPorIdAsync(int id)
        {
            var m = await _monedaRepository.GetByIdAsync(id);
            if (m == null) return null;

            return new MonedaDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                CodigoIso = m.CodigoIso,
                Simbolo = m.Simbolo,
                EsMonedaLocal = m.EsMonedaLocal,
                Estado = m.Estado
            };
        }

        public async Task<IEnumerable<MonedaDto>> ObtenerActivasAsync()
        {
            // Monedas activas para los nuevos formularios de los compañeros
            var monedasActivas = await _monedaRepository.FindAsync(m => m.Estado == true);
            return monedasActivas.Select(m => new MonedaDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                CodigoIso = m.CodigoIso,
                Simbolo = m.Simbolo,
                EsMonedaLocal = m.EsMonedaLocal,
                Estado = m.Estado
            });
        }

        public async Task CrearAsync(MonedaDto monedaDto)
        {
            // Validaciones básicas de campos requeridos
            if (string.IsNullOrWhiteSpace(monedaDto.Nombre))
                throw new BusinessException("El nombre de la moneda es requerido.");

            if (string.IsNullOrWhiteSpace(monedaDto.CodigoIso))
                throw new BusinessException("El código ISO es requerido.");

            if (string.IsNullOrWhiteSpace(monedaDto.Simbolo))
                throw new BusinessException("El símbolo es requerido.");

            // Código ISO debe tener exactamente 3 caracteres y en Mayúsculas
            string isoFormateado = monedaDto.CodigoIso.Trim().ToUpper();
            if (isoFormateado.Length != 3)
                throw new BusinessException("El código ISO debe tener exactamente 3 caracteres.");

            // No permitir duplicados de Código ISO
            var existenteIso = await _monedaRepository.FindAsync(m => m.CodigoIso == isoFormateado);
            if (existenteIso.Any())
                throw new BusinessException("Ya existe una moneda registrada con este código ISO.");

            // Solo puede existir una moneda local activa en el sistema
            if (monedaDto.EsMonedaLocal)
            {
                var existenteLocal = await _monedaRepository.FindAsync(m => m.EsMonedaLocal == true);
                if (existenteLocal.Any())
                    throw new BusinessException("Ya existe una moneda configurada como moneda local. Solo puede existir una moneda local en el sistema.");
            }

            var nuevaMoneda = new Moneda
            {
                Nombre = monedaDto.Nombre.Trim(),
                CodigoIso = isoFormateado,
                Simbolo = monedaDto.Simbolo.Trim(),
                EsMonedaLocal = monedaDto.EsMonedaLocal,
                Estado = monedaDto.Estado
            };

            await _monedaRepository.AddAsync(nuevaMoneda);
            await _monedaRepository.SaveAsync();
        }

        public async Task EditarAsync(MonedaDto monedaDto)
        {
            var monedaExistente = await _monedaRepository.GetByIdAsync(monedaDto.Id);
            if (monedaExistente == null)
                throw new BusinessException("La moneda a editar no existe.");

            if (string.IsNullOrWhiteSpace(monedaDto.Nombre))
                throw new BusinessException("El nombre de la moneda es requerido.");

            if (string.IsNullOrWhiteSpace(monedaDto.CodigoIso))
                throw new BusinessException("El código ISO es requerido.");

            if (string.IsNullOrWhiteSpace(monedaDto.Simbolo))
                throw new BusinessException("El símbolo es requerido.");

            string isoFormateado = monedaDto.CodigoIso.Trim().ToUpper();
            if (isoFormateado.Length != 3)
                throw new BusinessException("El código ISO debe tener exactamente 3 caracteres.");

            // Validar duplicidad de código ISO con otras monedas
            var duplicadoIso = await _monedaRepository.FindAsync(m => m.CodigoIso == isoFormateado && m.Id != monedaDto.Id);
            if (duplicadoIso.Any())
                throw new BusinessException("Ya existe una moneda registrada con este código ISO.");

            // Si el código ISO cambia, verificar si ya se usó en otros módulos para congelarlo
            if (monedaExistente.CodigoIso != isoFormateado)
            {
                bool estaEnUso = await _monedaRepository.RelacionesActivasAsync(monedaDto.Id);
                if (estaEnUso)
                {
                    throw new BusinessException("No se puede modificar el código ISO de esta moneda porque ya está siendo utilizada en registros del sistema.");
                }
            }

            // Solo puede existir una moneda local activa
            if (monedaDto.EsMonedaLocal && !monedaExistente.EsMonedaLocal)
            {
                var existenteLocal = await _monedaRepository.FindAsync(m => m.EsMonedaLocal == true && m.Id != monedaDto.Id);
                if (existenteLocal.Any())
                    throw new BusinessException("Ya existe una moneda configurada como moneda local. Solo puede existir una moneda local en el sistema.");
            }

            // No permitir inactivar la moneda local si tiene históricos vinculados
            if (monedaExistente.EsMonedaLocal && !monedaDto.Estado)
            {
                bool estaEnUso = await _monedaRepository.RelacionesActivasAsync(monedaDto.Id);
                if (estaEnUso)
                {
                    throw new BusinessException("No se puede inactivar la moneda local mientras existan registros que dependan de ella.");
                }
            }

            monedaExistente.Nombre = monedaDto.Nombre.Trim();
            monedaExistente.CodigoIso = isoFormateado;
            monedaExistente.Simbolo = monedaDto.Simbolo.Trim();
            monedaExistente.EsMonedaLocal = monedaDto.EsMonedaLocal;
            monedaExistente.Estado = monedaDto.Estado;

            _monedaRepository.Update(monedaExistente);
            await _monedaRepository.SaveAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var moneda = await _monedaRepository.GetByIdAsync(id);
            if (moneda == null) return;

            // Prohibido eliminar la moneda si está configurada como la Moneda Local
            if (moneda.EsMonedaLocal)
            {
                throw new BusinessException("No se puede eliminar la moneda local del sistema.");
            }

            //Validar que no tenga relaciones en ningún otro módulo
            bool estaEnUso = await _monedaRepository.RelacionesActivasAsync(id);
            if (estaEnUso)
            {
                throw new BusinessException("No se puede eliminar esta moneda porque está asociada a otros registros del sistema.");
            }

            _monedaRepository.Remove(moneda);
            await _monedaRepository.SaveAsync();
        }
    }
}