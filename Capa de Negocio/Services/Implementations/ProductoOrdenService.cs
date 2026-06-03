using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capa_de_Datos.Context;
using Capa_de_Datos.Entities;
using Capa_de_Datos.Enums;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capa_de_Negocio.Servicios
{
    public class ProductoOrdenService : IProductoOrdenService
    {
        private readonly IProductoOrdenRepository _productoOrdenRepository;
        private readonly IRepository<OrdenImportacion> _ordenRepository;
        private readonly IRepository<Producto> _productoRepository;
        private readonly ImportCostContext _context;

        public ProductoOrdenService(
            IProductoOrdenRepository productoOrdenRepository,
            IRepository<OrdenImportacion> ordenRepository,
            IRepository<Producto> productoRepository,
            ImportCostContext context)
        {
            _productoOrdenRepository = productoOrdenRepository;
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _context = context;
        }

        public async Task<OrdenResumenFobDto> ObtenerResumenOrdenAsync(int ordenId)
        {
            var orden = await _context.OrdenesImportacion
                .Include(o => o.Moneda)
                .Include(o => o.Proveedor)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null)
                throw new NotFoundException("La orden de importación seleccionada no existe.");

            var detalles = await _productoOrdenRepository.ObtenerPorOrdenIdAsync(ordenId);
            var gastos = await _context.GastosImportacion
                .Include(g => g.Moneda)
                .Where(g => g.OrdenId == ordenId)
                .ToListAsync();

            var resumen = new OrdenResumenFobDto
            {
                OrdenId = orden.Id,
                NumeroOrden = orden.NumeroOrden,
                EstadoOrden = orden.EstadoOrden,
                MonedaNombre = orden.Moneda?.Nombre ?? "No especificada",
                ProveedorNombre = orden.Proveedor?.Nombre ?? "No especificado"
            };

            foreach (var g in gastos)
            {
                resumen.Gastos.Add(new GastoImportacionDto
                {
                    Id = g.Id,
                    OrdenId = g.OrdenId,
                    TipoGasto = g.TipoGasto,
                    Monto = g.Monto,
                    MonedaId = g.MonedaId,
                    MonedaNombre = g.Moneda?.Nombre,
                    MetodoDistribucion = g.MetodoDistribucion,
                    FechaGasto = g.FechaGasto
                });
            }

            foreach (var d in detalles)
            {
                var productDto = new ProductoOrdenDto
                {
                    Id = d.Id,
                    OrdenImportacionId = d.OrdenId,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto?.Nombre,
                    CodigoReferencia = d.Producto?.CodigoReferencia,
                    Cantidad = d.Cantidad,
                    PrecioUnitarioFob = d.PrecioUnitarioFob,
                    MargenGananciaDeseado = d.MargenDeseado
                };

                
                if (d.Producto != null)
                {
                    productDto.PesoTotal = d.Cantidad * d.Producto.PesoUnitario;

                    if (d.Producto.Largo.HasValue && d.Producto.Ancho.HasValue && d.Producto.Alto.HasValue)
                    {
                        productDto.VolumenTotal = d.Cantidad * d.Producto.Largo.Value * d.Producto.Ancho.Value * d.Producto.Alto.Value;
                    }
                    else
                    {
                        productDto.VolumenTotal = null;
                        resumen.TieneProductosSinDimensiones = true;
                    }
                }

                resumen.Productos.Add(productDto);
            }

            // sumatorias para el FOB
            resumen.CantidadTotalProductos = resumen.Productos.Sum(p => p.Cantidad);
            resumen.FobTotalOrden = resumen.Productos.Sum(p => p.FobTotal);
            resumen.PesoTotalOrden = resumen.Productos.Sum(p => p.PesoTotal);
            resumen.VolumenTotalOrden = resumen.Productos.Sum(p => p.VolumenTotal ?? 0);

            return resumen;
        }

        public async Task<ProductoOrdenDto?> ObtenerPorIdAsync(int id)
        {
            var d = await _context.Set<ProductoOrden>()
                .Include(po => po.Producto)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (d == null) return null;

            return new ProductoOrdenDto
            {
                Id = d.Id,
                OrdenImportacionId = d.OrdenId,
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto?.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitarioFob = d.PrecioUnitarioFob,
                MargenGananciaDeseado = d.MargenDeseado
            };
        }

        public async Task AgregarProductoAOrdenAsync(ProductoOrdenDto dto)
        {
            var orden = await _ordenRepository.GetByIdAsync(dto.OrdenImportacionId);
            if (orden == null) throw new NotFoundException("La orden de importación no existe.");

            // Solo se pueden agregar productos a órdenes en estado Abierta
            if (orden.EstadoOrden != EstadoOrden.Abierta)
                throw new ValidationException($"No se pueden añadir productos a la orden porque se encuentra en estado: {orden.EstadoOrden}.");

            // No se puede agregar el mismo producto dos veces a una misma orden
            bool existe = await _productoOrdenRepository.ExisteProductoEnOrdenAsync(dto.OrdenImportacionId, dto.ProductoId);
            if (existe)
                throw new ConflictException("Este producto ya fue agregado a la orden. Si desea modificar la cantidad, precio o margen, debe editar el producto ya agregado.");

            var producto = await _productoRepository.GetByIdAsync(dto.ProductoId);
            if (producto == null) throw new NotFoundException("El producto seleccionado no existe.");

            // Solo se deben mostrar y permitir productos activos
            if (!producto.Estado)
                throw new ValidationException("El producto seleccionado no se puede agregar porque está inactivo.");

            ValidarValoresEstructurales(dto);

            var nuevoDetalle = new ProductoOrden
            {
                OrdenId = dto.OrdenImportacionId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitarioFob = dto.PrecioUnitarioFob,
                MargenDeseado = dto.MargenGananciaDeseado
            };

            await _productoOrdenRepository.AddAsync(nuevoDetalle);
            await _context.SaveChangesAsync();
        }

        public async Task EditarProductoEnOrdenAsync(ProductoOrdenDto dto)
        {
            var dRealPhysical = await _context.Set<ProductoOrden>().FindAsync(dto.Id);
            if (dRealPhysical == null) throw new NotFoundException("El registro de producto a editar no existe.");

            var orden = await _ordenRepository.GetByIdAsync(dRealPhysical.OrdenId);
            if (orden == null) throw new NotFoundException("La orden de importación no existe.");

            // Bloqueo estricto por estados
            if (orden.EstadoOrden == EstadoOrden.Calculada || orden.EstadoOrden == EstadoOrden.Cerrada || orden.EstadoOrden == EstadoOrden.Cancelada)
                throw new ValidationException($"No se puede editar este producto porque la orden ya fue {orden.EstadoOrden}.");

            ValidarValoresEstructurales(dto);

            dRealPhysical.Cantidad = dto.Cantidad;
            dRealPhysical.PrecioUnitarioFob = dto.PrecioUnitarioFob;
            dRealPhysical.MargenDeseado = dto.MargenGananciaDeseado;

            _context.Set<ProductoOrden>().Update(dRealPhysical);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarProductoDeOrdenAsync(int id)
        {
            var dRealPhysical = await _context.Set<ProductoOrden>().FindAsync(id);
            if (dRealPhysical == null) return;

            var orden = await _ordenRepository.GetByIdAsync(dRealPhysical.OrdenId);
            if (orden == null) throw new NotFoundException("La orden de importación no existe.");

            // Bloqueo de eliminación por estados
            if (orden.EstadoOrden == EstadoOrden.Calculada || orden.EstadoOrden == EstadoOrden.Cerrada || orden.EstadoOrden == EstadoOrden.Cancelada)
                throw new ValidationException($"No se puede eliminar este producto porque la orden ya fue {orden.EstadoOrden}.");

            _context.Set<ProductoOrden>().Remove(dRealPhysical);
            await _context.SaveChangesAsync();
        }

        private void ValidarValoresEstructurales(ProductoOrdenDto dto)
        {
            if (dto.Cantidad <= 0)
                throw new ValidationException("La cantidad debe ser mayor que 0.");

            if (dto.PrecioUnitarioFob <= 0)
                throw new ValidationException("El precio unitario FOB debe ser mayor que 0.");

            if (dto.MargenGananciaDeseado < 0 || dto.MargenGananciaDeseado >= 100)
                throw new ValidationException("El margen de ganancia deseado debe ser mayor o igual que 0 y menor que 100.");
        }

        public async Task<IEnumerable<ProductoOrdenDto>> ObtenerProductosActivosAsync()
        {
            var todosProductos = await _productoRepository.GetAllAsync();
            
            // Filtramos los productos activos y los proyectamos a DTOs
            return todosProductos
                .Where(p => p.Estado)
                .Select(p => new ProductoOrdenDto
                {
                    ProductoId = p.Id,
                    NombreProducto = p.Nombre
                })
                .ToList();
        }
    }
}