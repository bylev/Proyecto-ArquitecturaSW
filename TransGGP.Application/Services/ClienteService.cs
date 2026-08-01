using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

namespace TransGGP.Application.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository repoCliente)
        {
           _clienteRepository = repoCliente; // Inyección de dependencia del repositorio
        }

        public List<Cliente> ObtenerTodos()
        {
            return _clienteRepository.ObtenerTodos();
        }

        public Cliente? ObtenerPorId(int id)
        {
            return _clienteRepository.ObtenerPorId(id);
        }

        public Cliente RegistrarCliente(Cliente cliente)
        {
            Validar(cliente);
            _clienteRepository.Agregar(cliente);
            return cliente;
        }

        public void ActualizarCliente(Cliente cliente)
        {
            Validar(cliente);
            _clienteRepository.Actualizar(cliente);
        }

        public void EliminarCliente(int id)
        {
            _clienteRepository.Eliminar(id);
        }

        private void Validar(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new ValidacionException("El nombre del cliente es obligatorio.");
        }
}
}
