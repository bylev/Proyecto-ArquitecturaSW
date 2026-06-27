using Microsoft.Extensions.Logging;
using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.Infrastructure.Decorators
{
    /// <summary>
    /// PATRÓN DECORATOR (GOF - Estructural).
    ///
    /// Esta clase implementa la MISMA interfaz que decora (IClienteRepository)
    /// y ENVUELVE otra implementación (_inner). Antes y después de delegar la
    /// llamada al repositorio real, agrega comportamiento extra (logging),
    /// SIN modificar la clase original ClienteRepository.
    ///
    /// Beneficio: se puede activar/desactivar el logging solo cambiando el
    /// registro en la inyección de dependencias, sin tocar el código de negocio.
    /// </summary>
    public class ClienteRepositoryLoggingDecorator : IClienteRepository
    {
        private readonly IClienteRepository _inner;   // el objeto decorado (el repositorio real)
        private readonly ILogger<ClienteRepositoryLoggingDecorator> _logger;

        public ClienteRepositoryLoggingDecorator(
            IClienteRepository inner,
            ILogger<ClienteRepositoryLoggingDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public List<Cliente> ObtenerTodos()
        {
            _logger.LogInformation("[DECORATOR] Consultando todos los clientes...");
            var clientes = _inner.ObtenerTodos();          // delega al repositorio real
            _logger.LogInformation("[DECORATOR] Se obtuvieron {Count} clientes.", clientes.Count);
            return clientes;
        }

        public Cliente? ObtenerPorId(int id)
        {
            _logger.LogInformation("[DECORATOR] Buscando cliente con Id {Id}...", id);
            return _inner.ObtenerPorId(id);
        }

        public void Agregar(Cliente cliente)
        {
            _logger.LogInformation("[DECORATOR] Registrando nuevo cliente: {Nombre}", cliente.Nombre);
            _inner.Agregar(cliente);
            _logger.LogInformation("[DECORATOR] Cliente '{Nombre}' guardado correctamente.", cliente.Nombre);
        }

        public void Eliminar(int id)
        {
            _logger.LogInformation("[DECORATOR] Eliminando cliente con Id {Id}...", id);
            _inner.Eliminar(id);
            _logger.LogInformation("[DECORATOR] Cliente con Id {Id} eliminado.", id);
        }
    }
}
