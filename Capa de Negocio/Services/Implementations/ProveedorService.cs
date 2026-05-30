using Capa_de_Datos.Entities;
using Capa_de_Datos.Repositories.Interfaces;
using Capa_de_Negocio.DTOs;
using Capa_de_Negocio.Exceptions;
using Capa_de_Negocio.Interfaces;

namespace Capa_de_Negocio.Services;

public class ProveedorService : IProveedorService
{
    // conecto los repositorios para poder usar los datos 
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IRepository<OrdenImportacion> _ordenRepository;

    public ProveedorService(IProveedorRepository proveedorRepository, IRepository<OrdenImportacion> ordenRepository)
    {
        _proveedorRepository = proveedorRepository;
        _ordenRepository = ordenRepository;
    }

    // listar todos, trae la lista de proveedores para la pantalla principal
    public async Task<IEnumerable<ProveedorDTO>> ObtenerTodosAsync()
    {
        // busca todos los proveedores en la bd 
        var proveedores = await _proveedorRepository.GetAllAsync();

        // pasa los datos de la entidad al dto para mandarlos a la vista de forma limpia 
        return proveedores.Select(p => new ProveedorDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                PaisId = p.PaisId,
                // Si el proveedor tiene pais asignado, pone el nombre, si no, avisa
                PaisNombre = p.Pais != null ? p.Pais.Nombre : "No asignado",
                Correo = p.Correo,
                Telefono = p.Telefono,
                MonedaPrincipalId = p.MonedaPrincipalId,
                // Si tiene moneda asignada, pone el nombre para la tabla de Bootstrap
                MonedaNombre = p.MonedaPrincipal != null ? p.MonedaPrincipal.Nombre : "No asignada",
                Estado = p.Estado
            });
        }

        // busca por id, trae un solo proveedor cuando le da a Editar o Eliminar
        public async Task<ProveedorDTO?> ObtenerPorIdAsync(int id)
        {
            // Busca el proveedor por su numero de Id
            var p = await _proveedorRepository.GetByIdAsync(id);
            
            // Si no existe, devuelve nulo para que no se rompa el programa
            if (p == null) return null;

            // Si lo encuentra, llena el DTO con sus datos
            return new ProveedorDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                PaisId = p.PaisId,
                PaisNombre = p.Pais != null ? p.Pais.Nombre : "No asignado",
                Correo = p.Correo,
                Telefono = p.Telefono,
                MonedaPrincipalId = p.MonedaPrincipalId,
                MonedaNombre = p.MonedaPrincipal != null ? p.MonedaPrincipal.Nombre : "No asignada",
                Estado = p.Estado
            };
        }

        // guarda nuevo, registra un proveedor que se creo en el formulario
        public async Task<bool> CrearAsync(ProveedorDTO dto)
        {
            // Crea el nuevo registro para la base de datos con lo que escribio el usuario
            var nuevoProveedor = new Proveedor
            {
                Nombre = dto.Nombre,
                PaisId = dto.PaisId,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                MonedaPrincipalId = dto.MonedaPrincipalId,
                Estado = true // Al crearse debe empezar activo
            };

            // Guarda el nuevo proveedor en la base de datos
            await _proveedorRepository.AddAsync(nuevoProveedor);
            await _proveedorRepository.SaveAsync();
            return true;
        }

        // Guarda los cambios cuando edita un proveedor
        public async Task<bool> EditarAsync(ProveedorDTO dto)
        {
            // Busca el proveedor original para ver que datos tenia antes
            var proveedor = await _proveedorRepository.GetByIdAsync(dto.Id);
            if (proveedor == null) return false;

            // regla: Revisa si el proveedor ya se uso en alguna orden de compra
            bool tieneOrdenes = await TieneOrdenesAsociadasAsync(dto.Id);
            
            if (tieneOrdenes)
            {
                // Si ya tiene ordenes y el usuario intento cambiar el pais o la moneda, frena el proceso y manda el error
                if (proveedor.PaisId != dto.PaisId || proveedor.MonedaPrincipalId != dto.MonedaPrincipalId)
                {
                    throw new ReglasProvException("No se puede modificar el país de origen ni la moneda principal de este proveedor porque ya tiene órdenes de importación registradas.");
                }
            }

            // Datos que SIEMPRE se pueden cambiar 
            proveedor.Nombre = dto.Nombre;
            proveedor.Correo = dto.Correo;
            proveedor.Telefono = dto.Telefono;
            proveedor.Estado = dto.Estado;

            // Si el proveedor esta limpio y no tiene ordenes, si deja cambiar pais y moneda
            if (!tieneOrdenes)
            {
                proveedor.PaisId = dto.PaisId;
                proveedor.MonedaPrincipalId = dto.MonedaPrincipalId;
            }

            // Aplica los cambios en la base de datos
            _proveedorRepository.Update(proveedor);
            await _proveedorRepository.SaveAsync();
            return true;
        }

        // Borra el proveedor si cumple con los requisitos
        public async Task<bool> EliminarAsync(int id)
        {
            // busca el proveedor 
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null) return false;

            // regla: Si el proveedor ya se uso en una orden, no se puede borrar
            if (await TieneOrdenesAsociadasAsync(id))
            {
                throw new ReglasProvException("No se puede eliminar este proveedor porque tiene órdenes de importación registradas.");
            }

            // Si no tiene ordenes, lo borra por completo
            _proveedorRepository.Remove(proveedor);
            await _proveedorRepository.SaveAsync();
            return true;
        }

        // Cuenta rapido si este proveedor tiene historial de ordenes
        public async Task<bool> TieneOrdenesAsociadasAsync(int proveedorId)
        {
            var ordenes = await _ordenRepository.GetAllAsync();
            // Devuelve true si encuentra aunque sea una orden amarrada a este proveedor
            return ordenes.Any(o => o.ProveedorId == proveedorId);
        }
    }