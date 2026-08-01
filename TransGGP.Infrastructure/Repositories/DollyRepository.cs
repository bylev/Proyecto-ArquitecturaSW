using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class DollyRepository : IDollyRepository
    {
        private readonly ApplicationDbContext _context;

        public DollyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Dolly> ObtenerTodos()
        {
            return _context.Dollys.ToList();
        }

        public Dolly? ObtenerPorId(int id)
        {
            return _context.Dollys.Find(id);
        }

        public void Agregar(Dolly dolly)
        {
            _context.Dollys.Add(dolly);
            _context.SaveChanges();
        }

        public void Actualizar(Dolly dolly)
        {
            var existente = _context.Dollys.Find(dolly.Id);
            if (existente != null)
            {
                existente.Clave = dolly.Clave;
                existente.Placa = dolly.Placa;
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var dolly = _context.Dollys.Find(id);
            if (dolly != null)
            {
                _context.Dollys.Remove(dolly);
                _context.SaveChanges();
            }
        }
    }
}
