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
    public class LandedCostService : ILandedCostService
    {
        private readonly IOrdenImportacionRepository _ordenRepository;
        private readonly ILandedCostCalculoRepository _calculoRepository;
        private readonly ILandedCostDetalleRepository _detalleRepository;
        private readonly IMonedaService _monedaService;
        private readonly ITasasDeCambioService _tasasService;
        private readonly IConfiguracionImpuestoService _configImpuestoService;

        public LandedCostService(
            IOrdenImportacionRepository ordenRepository,
            ILandedCostCalculoRepository calculoRepository,
            ILandedCostDetalleRepository detalleRepository,
            IMonedaService monedaService,
            ITasasDeCambioService tasasService,
            IConfiguracionImpuestoService configImpuestoService)
        {
            _ordenRepository = ordenRepository;
            _calculoRepository = calculoRepository;
            _detalleRepository = detalleRepository;
            _monedaService = monedaService;
            _tasasService = tasasService;
            _configImpuestoService = configImpuestoService;
        }

        public async Task<LandedCostCalculoDto> CalcularAsync(int ordenId)
        {
            var orden = await _ordenRepository.GetWithDetailsAsync(ordenId);
            if (orden == null)
                throw new OrdenImportacionNotFoundException($"No se encontró la orden con ID {ordenId}");

            // Validaciones previas al cálculo
            await ValidarOrdenParaCalculoAsync(orden);

            return await EjecutarCalculoAsync(orden);
        }

        public async Task<LandedCostCalculoDto> GuardarCalculoOficialAsync(int ordenId)
        {
            var orden = await _ordenRepository.GetWithDetailsAsync(ordenId);
            if (orden == null)
                throw new OrdenImportacionNotFoundException($"No se encontró la orden con ID {ordenId}");

            if (orden.EstadoOrden == EstadoOrden.Calculada)
                throw new BusinessException("Esta orden ya tiene un cálculo oficial guardado.");

            var calculoDto = await CalcularAsync(ordenId);

            // Guardar cálculo oficial
            var calculoEntidad = new LandedCostCalculo
            {
                OrdenId = calculoDto.OrdenId,
                FechaCalculo = DateTime.Now,
                MonedaLocalUsadaId = calculoDto.MonedaLocalUsadaId,
                TasaCambioUsada = calculoDto.TasaCambioUsada,
                PorcentajeItbisGeneral = calculoDto.PorcentajeItbisGeneral,
                PorcentajeTasaAduanal = calculoDto.PorcentajeTasaAduanal,
                FobTotalOriginal = calculoDto.FobTotalOriginal,
                FobTotalLocal = calculoDto.FobTotalLocal,
                FleteTotalLocal = calculoDto.FleteTotalLocal,
                SeguroTotalLocal = calculoDto.SeguroTotalLocal,
                CifTotal = calculoDto.CifTotal,
                TotalArancel = calculoDto.TotalArancel,
                TotalImpuestoSelectivo = calculoDto.TotalImpuestoSelectivo,
                TotalTasaServicio = calculoDto.TotalTasaServicio,
                TotalItbis = calculoDto.TotalItbis,
                TotalGastosLocales = calculoDto.TotalGastosLocales,
                CostoTotalImportacion = calculoDto.CostoTotalImportacion,
                CantidadTotalImportada = calculoDto.CantidadTotalImportada
            };

            await _calculoRepository.AddAsync(calculoEntidad);
            await _calculoRepository.SaveAsync();

            foreach (var detalleDto in calculoDto.Detalles)
            {
                var detalleEntidad = new LandedCostDetalle
                {
                    CalculoId = calculoEntidad.Id,
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = detalleDto.Cantidad,
                    FobOriginal = detalleDto.FobOriginal,
                    FobLocal = detalleDto.FobLocal,
                    FleteAsignado = detalleDto.FleteAsignado,
                    SeguroAsignado = detalleDto.SeguroAsignado,
                    Cif = detalleDto.Cif,
                    PorcentajeArancelUsado = detalleDto.PorcentajeArancelUsado,
                    Arancel = detalleDto.Arancel,
                    PorcentajeSelectivoUsado = detalleDto.PorcentajeSelectivoUsado,
                    ImpuestoSelectivo = detalleDto.ImpuestoSelectivo,
                    TasaServicioAduanal = detalleDto.TasaServicioAduanal,
                    Itbis = detalleDto.Itbis,
                    GastosLocalesAsignados = detalleDto.GastosLocalesAsignados,
                    CostoTotalImportado = detalleDto.CostoTotalImportado,
                    CostoUnitarioImportado = detalleDto.CostoUnitarioImportado,
                    MargenDeseado = detalleDto.MargenDeseado,
                    PrecioVentaSugerido = detalleDto.PrecioVentaSugerido
                };
                await _detalleRepository.AddAsync(detalleEntidad);
            }

            await _detalleRepository.SaveAsync();

            // Cambiar estado de la orden
            orden.EstadoOrden = EstadoOrden.Calculada;
            _ordenRepository.Update(orden);
            await _ordenRepository.SaveAsync();

            calculoDto.Id = calculoEntidad.Id;
            return calculoDto;
        }

        public async Task<LandedCostCalculoDto?> ObtenerPorOrdenIdAsync(int ordenId)
        {
            var orden = await _ordenRepository.GetWithDetailsAsync(ordenId);
            if (orden == null || orden.LandedCostCalculo == null)
                return null;

            var calculo = orden.LandedCostCalculo;
            var monedaLocal = await _monedaService.ObtenerPorIdAsync(calculo.MonedaLocalUsadaId);

            var dto = new LandedCostCalculoDto
            {
                Id = calculo.Id,
                OrdenId = calculo.OrdenId,
                NumeroOrden = orden.NumeroOrden,
                EstadoOrden = orden.EstadoOrden,
                FechaCalculo = calculo.FechaCalculo,
                MonedaLocalUsadaId = calculo.MonedaLocalUsadaId,
                CodigoIsoMonedaLocal = monedaLocal?.CodigoIso ?? "N/A",
                TasaCambioUsada = calculo.TasaCambioUsada,
                PorcentajeItbisGeneral = calculo.PorcentajeItbisGeneral,
                PorcentajeTasaAduanal = calculo.PorcentajeTasaAduanal,
                FobTotalOriginal = calculo.FobTotalOriginal,
                FobTotalLocal = calculo.FobTotalLocal,
                FleteTotalLocal = calculo.FleteTotalLocal,
                SeguroTotalLocal = calculo.SeguroTotalLocal,
                CifTotal = calculo.CifTotal,
                TotalArancel = calculo.TotalArancel,
                TotalImpuestoSelectivo = calculo.TotalImpuestoSelectivo,
                TotalTasaServicio = calculo.TotalTasaServicio,
                TotalItbis = calculo.TotalItbis,
                TotalGastosLocales = calculo.TotalGastosLocales,
                CostoTotalImportacion = calculo.CostoTotalImportacion,
                CantidadTotalImportada = calculo.CantidadTotalImportada
            };

            // Fetch details
            var detalles = await _detalleRepository.FindAsync(d => d.CalculoId == calculo.Id);
            dto.Detalles = detalles.Select(d => new LandedCostDetalleDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto?.Nombre ?? "N/A",
                Cantidad = d.Cantidad,
                FobOriginal = d.FobOriginal,
                FobLocal = d.FobLocal,
                FleteAsignado = d.FleteAsignado,
                SeguroAsignado = d.SeguroAsignado,
                Cif = d.Cif,
                PorcentajeArancelUsado = d.PorcentajeArancelUsado,
                Arancel = d.Arancel,
                PorcentajeSelectivoUsado = d.PorcentajeSelectivoUsado,
                ImpuestoSelectivo = d.ImpuestoSelectivo,
                TasaServicioAduanal = d.TasaServicioAduanal,
                Itbis = d.Itbis,
                GastosLocalesAsignados = d.GastosLocalesAsignados,
                CostoTotalImportado = d.CostoTotalImportado,
                CostoUnitarioImportado = d.CostoUnitarioImportado,
                MargenDeseado = d.MargenDeseado,
                PrecioVentaSugerido = d.PrecioVentaSugerido
            }).ToList();

            return dto;
        }

        private async Task ValidarOrdenParaCalculoAsync(OrdenImportacion orden)
        {
            if (orden.EstadoOrden == EstadoOrden.Cerrada)
                throw new BusinessException("No se puede calcular una orden cerrada.");
            if (orden.EstadoOrden == EstadoOrden.Cancelada)
                throw new BusinessException("No se puede calcular una orden cancelada.");

            if (orden.ProductosOrden == null || !orden.ProductosOrden.Any())
                throw new BusinessException("La orden debe tener al menos un producto agregado.");

            if (!orden.GastosImportacion.Any(g => g.TipoGasto == TipoGasto.FleteInternacional))
                throw new BusinessException("No se puede calcular la orden porque no tiene registrado el gasto de flete internacional.");

            if (!orden.GastosImportacion.Any(g => g.TipoGasto == TipoGasto.SeguroInternacional))
                throw new BusinessException("No se puede calcular la orden porque no tiene registrado el gasto de seguro internacional.");

            var config = await _configImpuestoService.ObtenerConfiguracionActualAsync();
            if (config == null || config.Id == 0)
                throw new BusinessException("No se puede calcular landed cost si no existe una configuración de impuestos activa.");

            var monedas = await _monedaService.ObtenerTodasAsync();
            var monedaLocal = monedas.FirstOrDefault(m => m.EsMonedaLocal && m.Estado);
            if (monedaLocal == null)
                throw new BusinessException("No se debe permitir calcular landed cost si no existe una moneda local activa configurada en el mantenimiento de monedas.");

            // Validar tasa de cambio de la orden
            if (orden.MonedaId != monedaLocal.Id)
            {
                var tasaOrden = await BuscarTasaActivaAsync(orden.MonedaId, monedaLocal.Id, orden.FechaOrden);
                if (tasaOrden == null)
                    throw new BusinessException("No existe una tasa de cambio activa desde la moneda de la orden hacia la moneda local para la fecha de la orden.");
            }

            // Validar tasas de cambio de los gastos
            foreach (var gasto in orden.GastosImportacion)
            {
                if (gasto.MonedaId != monedaLocal.Id)
                {
                    var tasaGasto = await BuscarTasaActivaAsync(gasto.MonedaId, monedaLocal.Id, gasto.FechaGasto);
                    if (tasaGasto == null)
                        throw new BusinessException($"No existe una tasa de cambio activa desde la moneda del gasto ({gasto.Moneda?.CodigoIso}) hacia la moneda local para la fecha del gasto.");
                }

                // Validaciones de distribución
                switch (gasto.MetodoDistribucion)
                {
                    case MetodoDistribucion.PorPeso:
                        if (orden.ProductosOrden.Any(po => po.Producto == null || po.Producto.PesoUnitario <= 0))
                            throw new BusinessException("No se puede calcular el landed cost porque existen gastos distribuidos por peso y uno o más productos no tienen peso configurado.");
                        
                        decimal totalPeso = orden.ProductosOrden.Sum(po => po.Cantidad * po.Producto!.PesoUnitario);
                        if (totalPeso <= 0)
                            throw new BusinessException("No se puede calcular el landed cost porque el peso total de la orden es 0.");
                        break;
                    case MetodoDistribucion.PorVolumen:
                        if (orden.ProductosOrden.Any(po => po.Producto == null || (po.Producto.Largo ?? 0) <= 0 || (po.Producto.Ancho ?? 0) <= 0 || (po.Producto.Alto ?? 0) <= 0))
                            throw new BusinessException("No se puede calcular el landed cost porque existen gastos distribuidos por volumen y uno o más productos no tienen dimensiones configuradas.");
                        
                        decimal totalVol = orden.ProductosOrden.Sum(po => po.Cantidad * (po.Producto!.Largo ?? 0) * (po.Producto.Ancho ?? 0) * (po.Producto.Alto ?? 0));
                        if (totalVol <= 0)
                            throw new BusinessException("No se puede calcular el landed cost porque el volumen total de la orden es 0.");
                        break;
                    case MetodoDistribucion.PorValorFOB:
                        decimal totalFob = orden.ProductosOrden.Sum(po => po.Cantidad * po.PrecioUnitarioFob);
                        if (totalFob <= 0)
                            throw new BusinessException("No se puede calcular el landed cost porque el FOB total de la orden es 0.");
                        break;
                    case MetodoDistribucion.PorCantidad:
                        decimal totalCant = orden.ProductosOrden.Sum(po => po.Cantidad);
                        if (totalCant <= 0)
                            throw new BusinessException("No se puede calcular el landed cost porque la cantidad total de la orden es 0.");
                        break;
                }
            }

            // Validar categorías arancelarias y márgenes
            foreach (var po in orden.ProductosOrden)
            {
                if (po.Producto?.Categoria == null)
                    throw new BusinessException($"El producto {po.Producto?.Nombre} no tiene una categoría arancelaria válida.");
                
                if (po.MargenDeseado < 0 || po.MargenDeseado >= 100)
                    throw new BusinessException($"El margen de ganancia para el producto {po.Producto?.Nombre} debe ser mayor o igual que 0 y menor que 100.");
            }
        }

        private async Task<TasaCambioDto?> BuscarTasaActivaAsync(int monedaOrigenId, int monedaDestinoId, DateTime fecha)
        {
            var tasas = await _tasasService.GetTasasActivas();
            return tasas
                .Where(t => t.MonedaOrigenId == monedaOrigenId && t.MonedaDestinoId == monedaDestinoId && t.FechaVigencia.Date <= fecha.Date)
                .OrderByDescending(t => t.FechaVigencia)
                .FirstOrDefault();
        }

        private async Task<LandedCostCalculoDto> EjecutarCalculoAsync(OrdenImportacion orden)
        {
            var config = await _configImpuestoService.ObtenerConfiguracionActualAsync();
            var monedas = await _monedaService.ObtenerTodasAsync();
            var monedaLocal = monedas.First(m => m.EsMonedaLocal && m.Estado);

            var resultado = new LandedCostCalculoDto
            {
                OrdenId = orden.Id,
                NumeroOrden = orden.NumeroOrden,
                MonedaLocalUsadaId = monedaLocal.Id,
                CodigoIsoMonedaLocal = monedaLocal.CodigoIso,
                PorcentajeItbisGeneral = config.PorcentajeItbis ?? 0,
                PorcentajeTasaAduanal = config.PorcentajeTasaAduanal ?? 0
            };

            // Paso 1 & 2: FOB por producto y conversión
            decimal tasaCambioOrden = 1;
            if (orden.MonedaId != monedaLocal.Id)
            {
                var tasa = await BuscarTasaActivaAsync(orden.MonedaId, monedaLocal.Id, orden.FechaOrden);
                tasaCambioOrden = tasa!.ValorTasa;
            }
            resultado.TasaCambioUsada = tasaCambioOrden;

            foreach (var po in orden.ProductosOrden)
            {
                var detalle = new LandedCostDetalleDto
                {
                    ProductoId = po.ProductoId,
                    NombreProducto = po.Producto?.Nombre ?? "N/A",
                    Cantidad = po.Cantidad,
                    FobOriginal = po.PrecioUnitarioFob * po.Cantidad,
                    FobLocal = (po.PrecioUnitarioFob * po.Cantidad) * tasaCambioOrden,
                    MargenDeseado = po.MargenDeseado,
                    PorcentajeArancelUsado = po.Producto?.Categoria?.PorcentajeArancel ?? 0,
                    PorcentajeSelectivoUsado = (po.Producto?.Categoria?.AplicaSelectivo ?? false) 
                                                ? po.Producto.Categoria.PorcentajeSelectivo 
                                                : 0
                };
                resultado.Detalles.Add(detalle);
            }

            resultado.FobTotalOriginal = resultado.Detalles.Sum(d => d.FobOriginal);
            resultado.FobTotalLocal = resultado.Detalles.Sum(d => d.FobLocal);
            resultado.CantidadTotalImportada = resultado.Detalles.Sum(d => d.Cantidad);

            if (resultado.FobTotalLocal == 0)
                throw new BusinessException("El FOB total de la orden es 0, no se puede continuar con el cálculo.");

            // Paso 3: Convertir gastos a moneda local
            var gastosLocales = new List<(GastoImportacion Gasto, decimal MontoLocal)>();
            foreach (var g in orden.GastosImportacion)
            {
                decimal tasaGasto = 1;
                if (g.MonedaId != monedaLocal.Id)
                {
                    var tasa = await BuscarTasaActivaAsync(g.MonedaId, monedaLocal.Id, g.FechaGasto);
                    tasaGasto = tasa!.ValorTasa;
                }
                gastosLocales.Add((g, g.Monto * tasaGasto));
            }

            // Paso 4 & 5: Calcular totales base y prorratear gastos
            decimal pesoTotalOrden = orden.ProductosOrden.Sum(po => po.Cantidad * po.Producto!.PesoUnitario);
            decimal volumenTotalOrden = orden.ProductosOrden.Sum(po => po.Cantidad * (po.Producto!.Largo ?? 0) * (po.Producto.Ancho ?? 0) * (po.Producto.Alto ?? 0));
            decimal cantidadTotalOrden = resultado.CantidadTotalImportada;

            foreach (var item in gastosLocales)
            {
                var gasto = item.Gasto;
                decimal montoLocal = item.MontoLocal;

                foreach (var detalle in resultado.Detalles)
                {
                    var po = orden.ProductosOrden.First(p => p.ProductoId == detalle.ProductoId);
                    decimal factor = 0;

                    switch (gasto.MetodoDistribucion)
                    {
                        case MetodoDistribucion.PorValorFOB:
                            factor = detalle.FobLocal / resultado.FobTotalLocal;
                            break;
                        case MetodoDistribucion.PorPeso:
                            factor = (po.Cantidad * po.Producto!.PesoUnitario) / pesoTotalOrden;
                            break;
                        case MetodoDistribucion.PorVolumen:
                            decimal volProd = po.Cantidad * (po.Producto!.Largo ?? 0) * (po.Producto.Ancho ?? 0) * (po.Producto.Alto ?? 0);
                            factor = volProd / volumenTotalOrden;
                            break;
                        case MetodoDistribucion.PorCantidad:
                            factor = po.Cantidad / cantidadTotalOrden;
                            break;
                    }

                    decimal montoAsignado = montoLocal * factor;

                    if (gasto.TipoGasto == TipoGasto.FleteInternacional)
                        detalle.FleteAsignado += montoAsignado;
                    else if (gasto.TipoGasto == TipoGasto.SeguroInternacional)
                        detalle.SeguroAsignado += montoAsignado;
                    else
                        detalle.GastosLocalesAsignados += montoAsignado;
                }
            }

            // Paso 6-14: Cálculos finales por producto
            foreach (var detalle in resultado.Detalles)
            {
                var po = orden.ProductosOrden.First(p => p.ProductoId == detalle.ProductoId);
                var cat = po.Producto!.Categoria!;

                // Paso 7: CIF
                detalle.Cif = detalle.FobLocal + detalle.FleteAsignado + detalle.SeguroAsignado;

                // Paso 8: Arancel
                detalle.Arancel = detalle.Cif * (detalle.PorcentajeArancelUsado / 100);

                // Paso 9: Impuesto Selectivo
                detalle.ImpuestoSelectivo = detalle.Cif * (detalle.PorcentajeSelectivoUsado / 100);

                // Paso 10: Tasa Servicio Aduanal
                detalle.TasaServicioAduanal = detalle.Cif * (resultado.PorcentajeTasaAduanal / 100);

                // Paso 11: ITBIS
                if (cat.AplicaItbis)
                {
                    decimal baseItbis = detalle.Cif + detalle.Arancel + detalle.ImpuestoSelectivo + detalle.TasaServicioAduanal;
                    detalle.Itbis = baseItbis * (resultado.PorcentajeItbisGeneral / 100);
                }
                else
                {
                    detalle.Itbis = 0;
                }

                // Paso 12: Costo Total Importado
                detalle.CostoTotalImportado = detalle.Cif + detalle.Arancel + detalle.ImpuestoSelectivo + detalle.TasaServicioAduanal + detalle.Itbis + detalle.GastosLocalesAsignados;

                // Paso 13: Costo Unitario
                if (detalle.Cantidad > 0)
                    detalle.CostoUnitarioImportado = detalle.CostoTotalImportado / detalle.Cantidad;

                // Paso 14: Precio Venta Sugerido
                if (detalle.MargenDeseado < 100)
                {
                    detalle.PrecioVentaSugerido = detalle.CostoUnitarioImportado / (1 - (detalle.MargenDeseado / 100));
                }
            }

            // Totales del resultado
            resultado.FleteTotalLocal = resultado.Detalles.Sum(d => d.FleteAsignado);
            resultado.SeguroTotalLocal = resultado.Detalles.Sum(d => d.SeguroAsignado);
            resultado.CifTotal = resultado.Detalles.Sum(d => d.Cif);
            resultado.TotalArancel = resultado.Detalles.Sum(d => d.Arancel);
            resultado.TotalImpuestoSelectivo = resultado.Detalles.Sum(d => d.ImpuestoSelectivo);
            resultado.TotalTasaServicio = resultado.Detalles.Sum(d => d.TasaServicioAduanal);
            resultado.TotalItbis = resultado.Detalles.Sum(d => d.Itbis);
            resultado.TotalGastosLocales = resultado.Detalles.Sum(d => d.GastosLocalesAsignados);
            resultado.CostoTotalImportacion = resultado.Detalles.Sum(d => d.CostoTotalImportado);

            return resultado;
        }
    }
}
