using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;

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
            _clienteRepository.Agregar(cliente);
            return cliente;
        } 

        public void EliminarCliente(int id)
        {
            _clienteRepository.Eliminar(id);
        }
}
}
