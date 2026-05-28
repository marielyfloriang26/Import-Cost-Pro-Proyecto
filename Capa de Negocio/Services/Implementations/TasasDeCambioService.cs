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

        public async Task<IEnumerable<TasaCambioDto>> GetTasas()
        {
            var tasas = await _repository.GetAllAsync();
            var monedas = await _monedaService.ObtenerTodasAsync();

            return tasas.Select(t => new TasaCambioDto
            {
                IdTasaCambio = t.Id,
                MonedaOrigenId = t.MonedaOrigenId,
                MonedaOrigenNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaOrigenId)?.Nombre ?? "N/A",
                MonedaDestinoId = t.MonedaDestinoId,
                MonedaDestinoNombre = monedas.FirstOrDefault(m => m.Id == t.MonedaDestinoId)?.Nombre ?? "N/A",
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
                throw new TasaCambioNotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }

            var monedaOrigen = await _monedaService.ObtenerPorIdAsync(tasa.MonedaOrigenId);
            var monedaDestino = await _monedaService.ObtenerPorIdAsync(tasa.MonedaDestinoId);

            return new TasaCambioDto
            {
                IdTasaCambio = tasa.Id,
                MonedaOrigenId = tasa.MonedaOrigenId,
                MonedaOrigenNombre = monedaOrigen?.Nombre ?? "N/A",
                MonedaDestinoId = tasa.MonedaDestinoId,
                MonedaDestinoNombre = monedaDestino?.Nombre ?? "N/A",
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
                throw new TasaCambioNotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }

            bool hasBeenUsedInLandedCost = await _repository.HasBeenUsedInLandedCostAsync(tasaExistente);

            if (hasBeenUsedInLandedCost)
            {
                if (tasaExistente.MonedaOrigenId != tasaDto.MonedaOrigenId ||
                    tasaExistente.MonedaDestinoId != tasaDto.MonedaDestinoId ||
                    tasaExistente.ValorTasa != tasaDto.ValorTasa ||
                    tasaExistente.FechaVigencia.Date != tasaDto.FechaVigencia.Date)
                {
                    throw new TasaCambioLockedException("Esta tasa de cambio ya fue utilizada en un calculo oficial de landed cost y no puede ser modificada.");
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
                throw new TasaCambioNotFoundException($"No se encontró una tasa de cambio con el ID {id}");
            }
            bool hasBeenUsedInLandedCost = await _repository.HasBeenUsedInLandedCostAsync(tasaExistente);

            if(hasBeenUsedInLandedCost)
            {
                throw new TasaCambioLockedException("Esta tasa de cambio ya fue utilizada en un calculo oficial de landed cost y no puede ser eliminada.");
            }

            _repository.Remove(tasaExistente);
            await _repository.SaveAsync();
            return true;
        }

        private async Task ValidarTasaCambio(CrearTasaCambioDto tasaDto, int id = 0)
        {
            if (tasaDto.MonedaOrigenId == tasaDto.MonedaDestinoId)
            {
                throw new SameCurrencyNotAllowedException("La moneda origen no puede ser igual a la moneda destino.");
            }

            var monedaOrigen = await _monedaService.ObtenerPorIdAsync(tasaDto.MonedaOrigenId);
            var monedaDestino = await _monedaService.ObtenerPorIdAsync(tasaDto.MonedaDestinoId);

            if (monedaOrigen == null)
            {
                throw new CurrencyNotFoundException($"No se encontró una moneda con el ID {tasaDto.MonedaOrigenId}");
            }

            if (!monedaOrigen.Estado)
            {
                throw new InactiveCurrencyException($"La moneda de origen con ID {tasaDto.MonedaOrigenId} no está activa");
            }

            if (monedaDestino == null)
            {
                throw new CurrencyNotFoundException($"No se encontró una moneda con el ID {tasaDto.MonedaDestinoId}");
            }

            if (!monedaDestino.Estado)
            {
                throw new InactiveCurrencyException($"La moneda de destino con ID {tasaDto.MonedaDestinoId} no está activa");
            }

            if (!monedaDestino.EsMonedaLocal)
            {
                throw new LocalCurrencyRequiredException("La moneda destino debe ser local.");
            }

            if (tasaDto.ValorTasa <= 0)
            {
                throw new InvalidExchangeRateValueException("El valor de la tasa de cambio debe ser mayor a cero");
            }

            var tasasDuplicadas = await _repository.FindAsync(t =>
                t.Id != id &&
                t.MonedaOrigenId == tasaDto.MonedaOrigenId &&
                t.MonedaDestinoId == tasaDto.MonedaDestinoId &&
                t.FechaVigencia.Date == tasaDto.FechaVigencia.Date &&
                t.Estado == true);

            if (tasasDuplicadas.Any())
            {
                throw new DuplicateExchangeRateException("Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia.");
            }
        }
    }
}