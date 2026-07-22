using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class UnidadRepository : IUnidadRepository
    {
        private readonly ApplicationDbContext _context;

        public UnidadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Unidad> ObtenerTodos()
        {
            return _context.Unidades.ToList();
        }

        public Unidad? ObtenerPorId(int id)
        {
            return _context.Unidades.Find(id);
        }

        public void Agregar(Unidad unidad)
        {
            _context.Unidades.Add(unidad);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var unidad = _context.Unidades.Find(id);
            if (unidad != null)
            {
                _context.Unidades.Remove(unidad);
                _context.SaveChanges();
            }
        }
    }
}
