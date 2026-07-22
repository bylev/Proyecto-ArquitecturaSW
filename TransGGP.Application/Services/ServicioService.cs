using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;

namespace TransGGP.Application.Services
{
    public class ServicioService
    {
        private readonly IServicioRepository _servicioRepository;

        public ServicioService(IServicioRepository servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        public List<Servicio> ObtenerTodos() => _servicioRepository.ObtenerTodos();

        public Servicio? ObtenerPorId(int id) => _servicioRepository.ObtenerPorId(id);

        public Servicio RegistrarServicio(Servicio servicio)
        {
            _servicioRepository.Agregar(servicio);
            return servicio;
        }

        public void ActualizarServicio(Servicio servicio) => _servicioRepository.Actualizar(servicio);

        public void EliminarServicio(int id) => _servicioRepository.Eliminar(id);
    }
}
