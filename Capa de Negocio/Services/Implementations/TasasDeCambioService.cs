using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Services.Interfaces;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Implementations
{
    public class TasasDeCambioService : ITasasDeCambioService
    {
        private readonly ITasaCambioRepository _repository;
        private readonly IMonedaService _monedaService;

        public TasasDeCambioService(ITasaCambioRepository repository, IMonedaService monedaService)
        {
            _repository = repository;
            _monedaService = monedaService;
        }

        public async Task<IEnumerable<TasaCambioDto>> GetTasasActivas()
        {
            var tasas = await _repository.GetAllAsync();
            return tasas.Where(t => t.Estado).Select(t => new TasaCambioDto
            {
                IdTasaCambio = t.Id,
                MonedaOrigenId = t.MonedaOrigenId,
                MonedaOrigenNombre = t.MonedaOrigen?.Nombre ?? "N/A",
                MonedaDestinoId = t.MonedaDestinoId,
                MonedaDestinoNombre = t.MonedaDestino?.Nombre ?? "N/A",
                ValorTasa = t.ValorTasa,
                FechaVigencia = t.FechaVigencia,
                Estado = t.Estado
            });
        }

        public async Task<IEnumerable<TasaCambioDto>> GetTasas()
        {
            var tasas = await _repository.GetAllAsync();

            return tasas.Select(t => new TasaCambioDto
            {
                IdTasaCambio = t.Id,
                MonedaOrigenId = t.MonedaOrigenId,
                MonedaOrigenNombre = t.MonedaOrigen?.Nombre ?? "N/A",
                MonedaDestinoId = t.MonedaDestinoId,
                MonedaDestinoNombre = t.MonedaDestino?.Nombre ?? "N/A",
                ValorTasa = t.ValorTasa,
                FechaVigencia = t.FechaVigencia,
                Estado = t.Estado
            });
        }

        public async Task<TasaCambioDto> GetTasaById(int id)
        {
            var tasa = await _repository.GetByIdAsync(id);
            if (tasa == null)
            {
                throw new NotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }

            return new TasaCambioDto
            {
                IdTasaCambio = tasa.Id,
                MonedaOrigenId = tasa.MonedaOrigenId,
                MonedaOrigenNombre = tasa.MonedaOrigen?.Nombre ?? "N/A",
                MonedaDestinoId = tasa.MonedaDestinoId,
                MonedaDestinoNombre = tasa.MonedaDestino?.Nombre ?? "N/A",
                ValorTasa = tasa.ValorTasa,
                FechaVigencia = tasa.FechaVigencia,
                Estado = tasa.Estado
            };
        }

        public async Task<TasaCambioDto> CreateTasa(CrearTasaCambioDto tasaDto)
        {
            await ValidarTasaCambio(tasaDto);
            TasaCambio nuevaTasaCambio = new TasaCambio()
            {
                MonedaOrigenId = tasaDto.MonedaOrigenId,
                MonedaDestinoId = tasaDto.MonedaDestinoId,
                ValorTasa = tasaDto.ValorTasa,
                FechaVigencia = tasaDto.FechaVigencia,
                Estado = tasaDto.Estado
            };

            await _repository.AddAsync(nuevaTasaCambio);
            await _repository.SaveAsync();

            return await GetTasaById(nuevaTasaCambio.Id);
        }

        public async Task<TasaCambioDto> UpdateTasa(int id, TasaCambioDto tasaDto)
        {
            var tasaExistente = await _repository.GetByIdAsync(id);
            if (tasaExistente == null)
            {
                throw new NotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }

            bool hasBeenUsedInLandedCost = await _repository.HasBeenUsedInLandedCostAsync(tasaExistente);

            if (hasBeenUsedInLandedCost)
            {
                if (tasaExistente.MonedaOrigenId != tasaDto.MonedaOrigenId ||
                    tasaExistente.MonedaDestinoId != tasaDto.MonedaDestinoId ||
                    tasaExistente.ValorTasa != tasaDto.ValorTasa ||
                    tasaExistente.FechaVigencia.Date != tasaDto.FechaVigencia.Date)
                {
                    throw new ValidationException("Esta tasa de cambio ya fue utilizada en un calculo oficial de landed cost y no puede ser modificada.");
                }
            }

            await ValidarTasaCambio(new CrearTasaCambioDto
            {
                MonedaOrigenId = tasaDto.MonedaOrigenId,
                MonedaDestinoId = tasaDto.MonedaDestinoId,
                ValorTasa = tasaDto.ValorTasa,
                FechaVigencia = tasaDto.FechaVigencia,
                Estado = tasaDto.Estado
            }, id);


            tasaExistente.MonedaOrigenId = tasaDto.MonedaOrigenId;
            tasaExistente.MonedaDestinoId = tasaDto.MonedaDestinoId;
            tasaExistente.ValorTasa = tasaDto.ValorTasa;
            tasaExistente.FechaVigencia = tasaDto.FechaVigencia;
            tasaExistente.Estado = tasaDto.Estado;

            _repository.Update(tasaExistente);
            await _repository.SaveAsync();

            return await GetTasaById(tasaExistente.Id);
        }

        public async Task<bool> DeleteTasa(int id)
        {
            var tasaExistente = await _repository.GetByIdAsync(id);
            if (tasaExistente == null)
            {
                throw new NotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }
            bool hasBeenUsedInLandedCost = await _repository.HasBeenUsedInLandedCostAsync(tasaExistente);

            if(hasBeenUsedInLandedCost)
            {
                throw new ValidationException("Esta tasa de cambio ya fue utilizada en un calculo oficial de landed cost y no puede ser eliminada.");
            }

            _repository.Remove(tasaExistente);
            await _repository.SaveAsync();
            return true;
        }

        private async Task ValidarTasaCambio(CrearTasaCambioDto tasaDto, int id = 0)
        {
            if (tasaDto.MonedaOrigenId == tasaDto.MonedaDestinoId)
            {
                throw new ConflictException("La moneda origen no puede ser igual a la moneda destino.");
            }

            var monedaOrigen = await _monedaService.ObtenerPorIdAsync(tasaDto.MonedaOrigenId);
            var monedaDestino = await _monedaService.ObtenerPorIdAsync(tasaDto.MonedaDestinoId);

            if (monedaOrigen == null)
            {
                throw new NotFoundException($"No se encontró una moneda con el ID {tasaDto.MonedaOrigenId}");
            }

            if (!monedaOrigen.Estado)
            {
                throw new ValidationException($"La moneda de origen con ID {tasaDto.MonedaOrigenId} no está activa");
            }

            if (monedaDestino == null)
            {
                throw new NotFoundException($"No se encontró una moneda con el ID {tasaDto.MonedaDestinoId}");
            }

            if (!monedaDestino.Estado)
            {
                throw new ValidationException($"La moneda de destino con ID {tasaDto.MonedaDestinoId} no está activa");
            }

            if (!monedaDestino.EsMonedaLocal)
            {
                throw new ConflictException("La moneda destino debe ser local.");
            }

            if (tasaDto.ValorTasa <= 0)
            {
                throw new ValidationException("El valor de la tasa de cambio debe ser mayor a cero");
            }

            var tasasDuplicadas = await _repository.FindAsync(t =>
                t.Id != id &&
                t.MonedaOrigenId == tasaDto.MonedaOrigenId &&
                t.MonedaDestinoId == tasaDto.MonedaDestinoId &&
                t.FechaVigencia.Date == tasaDto.FechaVigencia.Date &&
                t.Estado == true);

            if (tasasDuplicadas.Any())
            {
                throw new ConflictException("Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia.");
            }
        }
    }
}