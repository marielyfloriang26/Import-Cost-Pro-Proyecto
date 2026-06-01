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

namespace Capa_de_Negocio.Services.Implementations;
    public class GastoImportacionService : IGastoImportacionService
    {
        private readonly IGastoImportacionRepository _gastoRepository;
        private readonly IOrdenImportacionRepository _ordenRepository; // Necesario para validar estados y productos
        private readonly IMonedaService _monedaService;
        private readonly ITasasDeCambioService _tasasService;

        public GastoImportacionService(
            IGastoImportacionRepository gastoRepository,
            IOrdenImportacionRepository ordenRepository,
            IMonedaService monedaService,
            ITasasDeCambioService tasasService)
        {
            _gastoRepository = gastoRepository;
            _ordenRepository = ordenRepository;
            _monedaService = monedaService;
            _tasasService = tasasService;
        }

        public async Task<IEnumerable<GastoImportacionDto>> ObtenerPorOrdenIdAsync(int ordenId)
        {
            var gastos = await _gastoRepository.FindAsync(g => g.OrdenId == ordenId);
            var monedas = await _monedaService.ObtenerTodasAsync();

            return gastos.Select(g => new GastoImportacionDto
            {
                Id = g.Id,
                OrdenId = g.OrdenId,
                OrdenCodigo = g.Orden?.NumeroOrden ?? "N/A",
                TipoGasto = g.TipoGasto,
                Monto = g.Monto,
                MonedaId = g.MonedaId,
                MonedaNombre = monedas.FirstOrDefault(m => m.Id == g.MonedaId)?.Nombre ?? "N/A",
                MonedaSimbolo = monedas.FirstOrDefault(m => m.Id == g.MonedaId)?.Simbolo ?? string.Empty,
                MetodoDistribucion = g.MetodoDistribucion,
                FechaGasto = g.FechaGasto
            });
        }

        public async Task<GastoImportacionDto?> ObtenerPorIdAsync(int id)
        {
            var g = await _gastoRepository.GetByIdAsync(id);
            if (g == null) return null;

            var moneda = await _monedaService.ObtenerPorIdAsync(g.MonedaId);

            return new GastoImportacionDto
            {
                Id = g.Id,
                OrdenId = g.OrdenId,
                OrdenCodigo = g.Orden?.NumeroOrden ?? "N/A",
                TipoGasto = g.TipoGasto,
                Monto = g.Monto,
                MonedaId = g.MonedaId,
                MonedaNombre = moneda?.Nombre ?? "N/A",
                MonedaSimbolo = moneda?.Simbolo ?? string.Empty,
                MetodoDistribucion = g.MetodoDistribucion,
                FechaGasto = g.FechaGasto
            };
        }

        public async Task RegistrarAsync(GastoImportacionDto dto)
        {
            // Validaciones basicas de negocio
            await ValidarReglasGastoAsync(dto);

            // Mapear DTO a Entidad
            var nuevoGasto = new GastoImportacion
            {
                OrdenId = dto.OrdenId,
                TipoGasto = dto.TipoGasto,
                Monto = dto.Monto,
                MonedaId = dto.MonedaId,
                MetodoDistribucion = dto.MetodoDistribucion,
                FechaGasto = dto.FechaGasto
            };

            await _gastoRepository.AddAsync(nuevoGasto);
            await _gastoRepository.SaveAsync();
        }

        public async Task EditarAsync(GastoImportacionDto dto)
        {
            var gastoExistente = await _gastoRepository.GetByIdAsync(dto.Id);
            if (gastoExistente == null)
                throw new BusinessException("El gasto de importación a editar no existe.");

            // No se permite cambiar la orden de importacion asociada en la edicion
            if (gastoExistente.OrdenId != dto.OrdenId)
                throw new BusinessException("No se puede modificar la orden de importación asociada a un gasto existente. Debe eliminarlo y crearlo de nuevo.");

            // Validaciones logicas pesadas
            await ValidarReglasGastoAsync(dto, isEdicion: true);

            // Actualizar campos permitidos
            gastoExistente.TipoGasto = dto.TipoGasto;
            gastoExistente.Monto = dto.Monto;
            gastoExistente.MonedaId = dto.MonedaId;
            gastoExistente.MetodoDistribucion = dto.MetodoDistribucion;
            gastoExistente.FechaGasto = dto.FechaGasto;

            _gastoRepository.Update(gastoExistente);
            await _gastoRepository.SaveAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null) return;

            // Valida estado de la orden antes de eliminar fisica o logicamente
            var orden = await _ordenRepository.GetByIdAsync(gasto.OrdenId);
            if (orden == null)
                throw new BusinessException("La orden asociada al gasto no existe.");

            // Prohibe acciones si la orden no esta Abierta
            if (orden.EstadoOrden != EstadoOrden.Abierta)
            {
                throw new BusinessException("No se puede eliminar este gasto porque la orden ya fue calculada, cerrada o cancelada.");
            }

            _gastoRepository.Remove(gasto);
            await _gastoRepository.SaveAsync();
        }

        // Motor Centralizado de Validaciones para Gastos de Importacion 
        private async Task ValidarReglasGastoAsync(GastoImportacionDto dto, bool isEdicion = false)
        {
            // Valida la existencia de la Orden
            var orden = await _ordenRepository.GetByIdAsync(dto.OrdenId);
            if (orden == null)
                throw new BusinessException("La orden de importación seleccionada no existe.");

            // Valida que el Estado de la Orden permita modificaciones (Solo "Abierta")
            if (orden.EstadoOrden != EstadoOrden.Abierta)
                throw new BusinessException("La orden seleccionada no está en un estado que permita registrar o modificar gastos.");

            // El monto debe ser estrictamente mayor a 0
            if (dto.Monto <= 0)
                throw new BusinessException("El monto debe ser mayor que 0.");

            // Valida existencia y estado de la Moneda
            var monedaGasto = await _monedaService.ObtenerPorIdAsync(dto.MonedaId);
            if (monedaGasto == null)
                throw new BusinessException("La moneda seleccionada no existe en el mantenimiento de monedas.");

            // En creacion exige moneda activa, En edicion se permite historica si ya la tenia
            if (!isEdicion && !monedaGasto.Estado)
                throw new BusinessException("La moneda seleccionada no está activa.");

            // Validaciones de tipos unicos (Flete y Seguro)
            var gastosExistentes = await _gastoRepository.FindAsync(g => g.OrdenId == dto.OrdenId);

            if (dto.TipoGasto == TipoGasto.FleteInternacional)
            {
                bool yaExisteFlete = gastosExistentes.Any(g => g.TipoGasto == TipoGasto.FleteInternacional && (!isEdicion || g.Id != dto.Id));
                if (yaExisteFlete)
                    throw new BusinessException("Ya existe un gasto de flete internacional registrado para esta orden.");
            }

            if (dto.TipoGasto == TipoGasto.SeguroInternacional)
            {
                bool yaExisteSeguro = gastosExistentes.Any(g => g.TipoGasto == TipoGasto.SeguroInternacional && (!isEdicion || g.Id != dto.Id));
                if (yaExisteSeguro)
                    throw new BusinessException("Ya existe un gasto de seguro internacional registrado para esta orden.");
            }

            // Valida Tasa de Cambio si la moneda no es la local
            if (!monedaGasto.EsMonedaLocal)
            {
                var tasasActivas = await _tasasService.GetTasasActivas();
                
                // Busca tasa cuya FechaVigencia sea menor o igual a la FechaGasto
                var tasaValida = tasasActivas
                    .Where(t => t.MonedaOrigenId == dto.MonedaId && t.FechaVigencia.Date <= dto.FechaGasto.Date)
                    .OrderByDescending(t => t.FechaVigencia) // Conseguir la mas reciente
                    .FirstOrDefault();

                if (tasaValida == null)
                    throw new BusinessException("No existe una tasa de cambio activa desde la moneda del gasto hacia la moneda local para la fecha del gasto.");
            }

            // Validaciones segun el Metodo de Distribucion y los Productos de la Orden
            if (orden.ProductosOrden == null || !orden.ProductosOrden.Any())
            {
                throw new BusinessException("La orden debe tener al menos un producto registrado para poder asociarle gastos.");
            }

            switch (dto.MetodoDistribucion)
            {
                case MetodoDistribucion.PorValorFOB:
                    decimal fobTotal = orden.ProductosOrden.Sum(po => po.Cantidad * po.PrecioUnitarioFob);
                    if (fobTotal <= 0)
                        throw new BusinessException("Si el método de distribución es Por valor FOB, el FOB total de la orden debe ser mayor que 0.");
                    break;

                case MetodoDistribucion.PorPeso:
                    // Validar que todos los productos enlazados tengan un peso unitario asignado mayor a 0
                    bool tieneProductosSinPeso = orden.ProductosOrden.Any(po => po.Producto == null || po.Producto.PesoUnitario <= 0);
                    if (tieneProductosSinPeso)
                        throw new BusinessException("Si el método de distribución es Por peso, todos los productos deben tener peso unitario mayor que 0.");
                    break;

                case MetodoDistribucion.PorVolumen:
                    // Valida dimensiones fisicas (Largo, Ancho, Alto) de cada producto de la orden
                    bool tieneProductosSinVolumen = orden.ProductosOrden.Any(po => 
                        po.Producto == null || 
                        (po.Producto.Largo ?? 0) <= 0 || 
                        (po.Producto.Ancho ?? 0) <= 0 || 
                        (po.Producto.Alto ?? 0) <= 0);
                    
                    if (tieneProductosSinVolumen)
                        throw new BusinessException("Si el método de distribución es Por volumen, todos los productos deben tener largo, ancho y alto mayores que 0.");
                    break;

                case MetodoDistribucion.PorCantidad:
                    decimal cantidadTotal = orden.ProductosOrden.Sum(po => po.Cantidad);
                    if (cantidadTotal <= 0)
                        throw new BusinessException("Si el método de distribución es Por cantidad, la cantidad total debe ser mayor que 0.");
                    break;
            }
        }
    }
