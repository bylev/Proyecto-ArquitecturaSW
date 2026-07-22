using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class OperadorRepository : IOperadorRepository
    {
        private readonly ApplicationDbContext _context;

        public OperadorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Operador> ObtenerTodos()
        {
            return _context.Operadores.ToList();
        }

        public Operador? ObtenerPorId(int id)
        {
            return _context.Operadores.Find(id);
        }

        public void Agregar(Operador operador)
        {
            _context.Operadores.Add(operador);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var operador = _context.Operadores.Find(id);
            if (operador != null)
            {
                _context.Operadores.Remove(operador);
                _context.SaveChanges();
            }
        }
    }
}
