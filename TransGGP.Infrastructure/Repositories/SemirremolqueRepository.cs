using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class SemirremolqueRepository : ISemirremolqueRepository
    {
        private readonly ApplicationDbContext _context;

        public SemirremolqueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Semirremolque> ObtenerTodos()
        {
            return _context.Semirremolques.ToList();
        }

        public Semirremolque? ObtenerPorId(int id)
        {
            return _context.Semirremolques.Find(id);
        }

        public void Agregar(Semirremolque semirremolque)
        {
            _context.Semirremolques.Add(semirremolque);
            _context.SaveChanges();
        }

        public void Actualizar(Semirremolque semirremolque)
        {
            var existente = _context.Semirremolques.Find(semirremolque.Id);
            if (existente != null)
            {
                existente.Clave = semirremolque.Clave;
                existente.Placa = semirremolque.Placa;
                existente.Tipo = semirremolque.Tipo;
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var semirremolque = _context.Semirremolques.Find(id);
            if (semirremolque != null)
            {
                _context.Semirremolques.Remove(semirremolque);
                _context.SaveChanges();
            }
        }
    }
}
