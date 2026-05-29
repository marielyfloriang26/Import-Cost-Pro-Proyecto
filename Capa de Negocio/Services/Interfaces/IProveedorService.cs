using Capa_de_Negocio.DTOs;

namespace Capa_de_Negocio.Interfaces;

public interface IProveedorService
{
    Task<ProveedorDTO?> ObtenerPorIdAsync(int id);
    
    // lista todos los proveedores para la pantalla inicial
    Task<IEnumerable<ProveedorDTO>> ObtenerTodosAsync();

    // crea un nuevo proveedor, viene activa por defecto
    Task<bool> CrearAsync(ProveedorDTO dTO);

    // edita un proveedor, controla que no se cambie pais ni moneda si tiene ordenes 
    Task<bool> EditarAsync(ProveedorDTO dTO);

    // elimina proveedor que falla si tiene ordenes asociadas
    Task<bool> EliminarAsync(int Id);

    // metodo que valida las reglas de negocio en el controlador si es necesario
    Task<bool> TieneOrdenesAsociadasAsync(int proveedorId);

}