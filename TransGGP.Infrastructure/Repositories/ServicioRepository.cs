using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly ApplicationDbContext _context;

        public ServicioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Servicio> ObtenerTodos()
        {
            return _context.Servicios.ToList();
        }

        public Servicio? ObtenerPorId(int id)
        {
            return _context.Servicios.Find(id);
        }

        public void Agregar(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            _context.SaveChanges();
        }

        public void Actualizar(Servicio servicio)
        {
            var existente = _context.Servicios.Find(servicio.Id);
            if (existente != null)
            {
                // Se copian los campos editables (no se toca Id ni FechaCreacion)
                existente.NumeroEmbarque = servicio.NumeroEmbarque;
                existente.NumeroRemision = servicio.NumeroRemision;
                existente.FolioFactura = servicio.FolioFactura;
                existente.FechaCarga = servicio.FechaCarga;
                existente.HoraCita = servicio.HoraCita;
                existente.FechaEntrega = servicio.FechaEntrega;
                existente.Origen = servicio.Origen;
                existente.Destino = servicio.Destino;
                existente.TipoCarga = servicio.TipoCarga;
                existente.Observaciones = servicio.Observaciones;
                existente.Estatus = servicio.Estatus;
                existente.ClienteId = servicio.ClienteId;
                existente.OperadorId = servicio.OperadorId;
                existente.UnidadId = servicio.UnidadId;
                existente.SemirremolqueId = servicio.SemirremolqueId;
                existente.DollyId = servicio.DollyId;
                existente.ConfiguracionId = servicio.ConfiguracionId;
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                _context.SaveChanges();
            }
        }
    }
}
