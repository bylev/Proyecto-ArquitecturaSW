using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class ConfiguracionRepository : IConfiguracionRepository
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Configuracion> ObtenerTodos()
        {
            return _context.Configuraciones.ToList();
        }

        public Configuracion? ObtenerPorId(int id)
        {
            return _context.Configuraciones.Find(id);
        }

        public void Agregar(Configuracion configuracion)
        {
            _context.Configuraciones.Add(configuracion);
            _context.SaveChanges();
        }

        public void Actualizar(Configuracion configuracion)
        {
            var existente = _context.Configuraciones.Find(configuracion.Id);
            if (existente != null)
            {
                existente.Nombre = configuracion.Nombre;
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var configuracion = _context.Configuraciones.Find(id);
            if (configuracion != null)
            {
                _context.Configuraciones.Remove(configuracion);
                _context.SaveChanges();
            }
        }
    }
}
