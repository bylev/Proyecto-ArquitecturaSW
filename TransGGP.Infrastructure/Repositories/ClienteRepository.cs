using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Cliente> ObtenerTodos()
        {
            return _context.Clientes.ToList();
        }

        public Cliente? ObtenerPorId(int id)
        {
            return _context.Clientes.Find(id);
        }

        public void Agregar(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }
    }
}
