using Capa_de_Datos.Entities;
using Capa_de_Datos.Enums;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Capa_de_Negocio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Services.Implementations
{
    public class OrdenImportacionService : IOrdenImportacionService
    {
        private readonly IOrdenImportacionRepository _repository;
        private readonly IImportadorService _importadorService;
        private readonly IProveedorService _proveedorService;
        private readonly IPaisService _paisService;
        private readonly IMonedaService _monedaService;

        public OrdenImportacionService(
            IOrdenImportacionRepository repository,
            IImportadorService importadorService,
            IProveedorService proveedorService,
            IPaisService paisService,
            IMonedaService monedaService)
        {
            _repository = repository;
            _importadorService = importadorService;
            _proveedorService = proveedorService;
            _paisService = paisService;
            _monedaService = monedaService;
        }

        public async Task<IEnumerable<OrdenImportacionDto>> ObtenerTodasAsync()
        {
            var ordenes = await _repository.GetAllWithDetailsAsync();
            return ordenes.Select(MapToDto);
        }

        public async Task<IEnumerable<OrdenImportacionDto>> ObtenerAbiertasAsync()
        {
            var ordenes = await _repository.GetAllWithDetailsAsync();
            return ordenes.Where(o => o.EstadoOrden == EstadoOrden.Abierta).Select(MapToDto);
        }

        public async Task<OrdenImportacionDto?> ObtenerPorIdAsync(int id)
        {
            var orden = await _repository.GetWithDetailsAsync(id);
            return orden != null ? MapToDto(orden) : null;
        }

        public async Task<OrdenImportacionDto> CrearAsync(CrearOrdenImportacionDto dto)
        {
            await ValidarOrdenNueva(dto);

            var nuevaOrden = new OrdenImportacion
            {
                NumeroOrden = dto.NumeroOrden.Trim(),
                ImportadorId = dto.ImportadorId,
                ProveedorId = dto.ProveedorId,
                PaisOrigenId = dto.PaisOrigenId,
                MonedaId = dto.MonedaId,
                FechaOrden = dto.FechaOrden,
                MedioTransporte = dto.MedioTransporte,
                EstadoOrden = EstadoOrden.Abierta
            };

            await _repository.AddAsync(nuevaOrden);
            await _repository.SaveAsync();

            return (await ObtenerPorIdAsync(nuevaOrden.Id))!;
        }

        public async Task<OrdenImportacionDto> EditarAsync(int id, OrdenImportacionDto dto)
        {
            var ordenExistente = await _repository.GetWithDetailsAsync(id);
            if (ordenExistente == null)
                throw new OrdenImportacionNotFoundException($"No se encontró la orden con ID {id}");

            ValidarEstadoEdicion(ordenExistente);
            await ValidarEdicionCamposCriticos(ordenExistente, dto);
            await ValidarCambioEntidadesActivas(ordenExistente, dto);

            ordenExistente.NumeroOrden = dto.NumeroOrden.Trim();
            ordenExistente.ImportadorId = dto.ImportadorId;
            ordenExistente.ProveedorId = dto.ProveedorId;
            ordenExistente.PaisOrigenId = dto.PaisOrigenId;
            ordenExistente.MonedaId = dto.MonedaId;
            ordenExistente.FechaOrden = dto.FechaOrden;
            ordenExistente.MedioTransporte = dto.MedioTransporte;

            _repository.Update(ordenExistente);
            await _repository.SaveAsync();

            return (await ObtenerPorIdAsync(id))!;
        }

        public async Task EliminarAsync(int id)
        {
            var orden = await _repository.GetWithDetailsAsync(id);
            if (orden == null) return;

            if (orden.EstadoOrden == EstadoOrden.Calculada || orden.EstadoOrden == EstadoOrden.Cerrada || orden.LandedCostCalculo != null)
            {
                throw new OrdenImportacionLockedException("No se puede eliminar esta orden porque ya tiene un cálculo oficial de landed cost o está cerrada.");
            }

            _repository.Remove(orden);
            await _repository.SaveAsync();
        }

        private async Task ValidarCambioEntidadesActivas(OrdenImportacion orden, OrdenImportacionDto dto)
        {
            if (dto.ImportadorId != orden.ImportadorId)
            {
                var ent = await _importadorService.ObtenerPorIdAsync(dto.ImportadorId);
                if (ent == null) throw new ImportadorNotFoundException($"El importador con ID {dto.ImportadorId} no existe.");
                if (!ent.Estado) throw new InactiveImportadorException("El nuevo importador seleccionado no está activo.");
            }

            if (dto.ProveedorId != orden.ProveedorId)
            {
                var ent = await _proveedorService.ObtenerPorIdAsync(dto.ProveedorId);
                if (ent == null) throw new ProveedorNotFoundException($"El proveedor con ID {dto.ProveedorId} no existe.");
                if (!ent.Estado) throw new InactiveProveedorException("El nuevo proveedor seleccionado no está activo.");
            }

            if (dto.PaisOrigenId != orden.PaisOrigenId)
            {
                var ent = await _paisService.ObtenerPorIdAsync(dto.PaisOrigenId);
                if (ent == null) throw new PaisNotFoundException($"El país con ID {dto.PaisOrigenId} no existe.");
                if (!ent.Estado) throw new InactivePaisException("El nuevo país seleccionado no está activo.");
            }

            if (dto.MonedaId != orden.MonedaId)
            {
                var ent = await _monedaService.ObtenerPorIdAsync(dto.MonedaId);
                if (ent == null) throw new CurrencyNotFoundException($"La moneda con ID {dto.MonedaId} no existe.");
                if (!ent.Estado) throw new InactiveCurrencyException("La nueva moneda seleccionada no está activa.");
            }
        }

        public async Task<bool> CambiarEstadoAsync(int id, EstadoOrden nuevoEstado)
        {
            var orden = await _repository.GetByIdAsync(id);
            if (orden == null) throw new OrdenImportacionNotFoundException($"Orden {id} no encontrada.");

            ValidarTransicionEstado(orden.EstadoOrden, nuevoEstado);

            orden.EstadoOrden = nuevoEstado;
            _repository.Update(orden);
            await _repository.SaveAsync();
            return true;
        }

        private async Task ValidarOrdenNueva(CrearOrdenImportacionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NumeroOrden))
                throw new OrdenImportacionException("El número de orden es requerido.");

            if (dto.NumeroOrden.Length > 30)
                throw new OrdenImportacionException("El número de orden no puede exceder los 30 caracteres.");

            var duplicado = await _repository.FindAsync(o => o.NumeroOrden == dto.NumeroOrden.Trim());
            if (duplicado.Any())
                throw new DuplicateOrderNumberException("Ya existe una orden de importación registrada con este número.");

            var importador = await _importadorService.ObtenerPorIdAsync(dto.ImportadorId);
            if (importador == null) throw new ImportadorNotFoundException($"El importador con ID {dto.ImportadorId} no existe.");
            if (!importador.Estado)
                throw new InactiveImportadorException("El importador seleccionado no está activo.");

            var proveedor = await _proveedorService.ObtenerPorIdAsync(dto.ProveedorId);
            if (proveedor == null) throw new ProveedorNotFoundException($"El proveedor con ID {dto.ProveedorId} no existe.");
            if (!proveedor.Estado)
                throw new InactiveProveedorException("El proveedor seleccionado no está activo.");

            var pais = await _paisService.ObtenerPorIdAsync(dto.PaisOrigenId);
            if (pais == null) throw new PaisNotFoundException($"El país con ID {dto.PaisOrigenId} no existe.");
            if (!pais.Estado)
                throw new InactivePaisException("El país seleccionado no está activo.");

            var moneda = await _monedaService.ObtenerPorIdAsync(dto.MonedaId);
            if (moneda == null) throw new CurrencyNotFoundException($"La moneda con ID {dto.MonedaId} no existe.");
            if (!moneda.Estado)
                throw new InactiveCurrencyException("La moneda seleccionada no está activa.");
        }

        private void ValidarEstadoEdicion(OrdenImportacion orden)
        {
            if (orden.EstadoOrden == EstadoOrden.Cerrada || orden.EstadoOrden == EstadoOrden.Cancelada)
                throw new OrdenImportacionLockedException("No se puede editar esta orden porque está cerrada o cancelada.");
        }

        private async Task ValidarEdicionCamposCriticos(OrdenImportacion orden, OrdenImportacionDto dto)
        {
            if (dto.NumeroOrden.Length > 30)
                throw new OrdenImportacionException("El número de orden no puede exceder los 30 caracteres.");

            if (orden.EstadoOrden == EstadoOrden.Calculada)
            {
                bool camposCriticosCambiaron = 
                    orden.ImportadorId != dto.ImportadorId ||
                    orden.ProveedorId != dto.ProveedorId ||
                    orden.PaisOrigenId != dto.PaisOrigenId ||
                    orden.MonedaId != dto.MonedaId ||
                    orden.FechaOrden.Date != dto.FechaOrden.Date ||
                    orden.MedioTransporte != dto.MedioTransporte;

                if (camposCriticosCambiaron)
                    throw new OrdenImportacionLockedException("No se pueden modificar estos datos porque la orden ya tiene un cálculo oficial de landed cost.");
            }

            if (orden.NumeroOrden != dto.NumeroOrden.Trim())
            {
                var duplicado = await _repository.FindAsync(o => o.NumeroOrden == dto.NumeroOrden.Trim() && o.Id != orden.Id);
                if (duplicado.Any())
                    throw new DuplicateOrderNumberException("Ya existe otra orden registrada con este número.");
            }
        }

        private void ValidarTransicionEstado(EstadoOrden actual, EstadoOrden nuevo)
        {
            bool esValida = false;

            if (actual == EstadoOrden.Abierta)
                esValida = nuevo == EstadoOrden.Calculada || nuevo == EstadoOrden.Cancelada;
            else if (actual == EstadoOrden.Calculada)
                esValida = nuevo == EstadoOrden.Cerrada || nuevo == EstadoOrden.Abierta; 
            else if (actual == EstadoOrden.Cerrada || actual == EstadoOrden.Cancelada)
                esValida = false;

            if (!esValida)
                throw new InvalidOrderStatusTransitionException($"Transición de estado de {actual} a {nuevo} no permitida.");
        }

        private OrdenImportacionDto MapToDto(OrdenImportacion o)
        {
            return new OrdenImportacionDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                ImportadorId = o.ImportadorId,
                ImportadorNombre = o.Importador?.NombreRazonSocial ?? "N/A",
                ProveedorId = o.ProveedorId,
                ProveedorNombre = o.Proveedor?.Nombre ?? "N/A",
                PaisOrigenId = o.PaisOrigenId,
                PaisOrigenNombre = o.PaisOrigen?.Nombre ?? "N/A",
                MonedaId = o.MonedaId,
                MonedaNombre = o.Moneda?.Nombre ?? "N/A",
                FechaOrden = o.FechaOrden,
                FechaCierre = o.FechaCierre,
                MedioTransporte = o.MedioTransporte,
                EstadoOrden = o.EstadoOrden,
                FobTotal = o.ProductosOrden?.Sum(p => p.Cantidad * p.PrecioUnitarioFob) ?? 0,
                TotalEstimadoLandedCost = o.LandedCostCalculo?.CostoTotalImportacion ?? 0
            };
        }
    }
}
