using Microsoft.Extensions.Logging;
using TransGGP.Application.Interfaces;
using TransGGP.Domain.Models;

namespace TransGGP.Infrastructure.Decorators
{

    public class ClienteRepositoryLoggingDecorator : IClienteRepository
    {
        private readonly IClienteRepository _inner;   
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
            _logger.LogInformation("[Cliente] Consultando todos los clientes...");
            var clientes = _inner.ObtenerTodos();          // delega al repositorio real
            _logger.LogInformation("[Cliente] Se obtuvieron {Count} clientes.", clientes.Count);
            return clientes;
        }

        public Cliente? ObtenerPorId(int id)
        {
            _logger.LogInformation("[Cliente] Buscando cliente con Id {Id}...", id);
            return _inner.ObtenerPorId(id);
        }

        public void Agregar(Cliente cliente)
        {
            _logger.LogInformation("[Cliente] Registrando nuevo cliente: {Nombre}", cliente.Nombre);
            _inner.Agregar(cliente);
            _logger.LogInformation("[Cliente] Cliente '{Nombre}' guardado correctamente.", cliente.Nombre);
        }

        public void Actualizar(Cliente cliente)
        {
            _logger.LogInformation("[Cliente] Actualizando cliente con Id {Id}...", cliente.Id);
            _inner.Actualizar(cliente);
            _logger.LogInformation("[Cliente] Cliente con Id {Id} actualizado.", cliente.Id);
        }

        public void Eliminar(int id)
        {
            _logger.LogInformation("[Cliente] Eliminando cliente con Id {Id}...", id);
            _inner.Eliminar(id);
            _logger.LogInformation("[Cliente] Cliente con Id {Id} eliminado.", id);
        }
    }
}
