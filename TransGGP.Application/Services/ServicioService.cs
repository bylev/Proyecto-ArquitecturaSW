using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

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
            Validar(servicio);
            _servicioRepository.Agregar(servicio);
            return servicio;
        }

        public void ActualizarServicio(Servicio servicio)
        {
            Validar(servicio);
            _servicioRepository.Actualizar(servicio);
        }

        private void Validar(Servicio servicio)
        {
            if (string.IsNullOrWhiteSpace(servicio.NumeroEmbarque))
                throw new ValidacionException("El número de embarque es obligatorio.");
            if (string.IsNullOrWhiteSpace(servicio.Origen))
                throw new ValidacionException("El origen es obligatorio.");
            if (string.IsNullOrWhiteSpace(servicio.Destino))
                throw new ValidacionException("El destino es obligatorio.");
            if (servicio.ClienteId <= 0)
                throw new ValidacionException("Debes seleccionar un cliente.");
            if (servicio.OperadorId <= 0)
                throw new ValidacionException("Debes seleccionar un operador.");
            if (servicio.UnidadId <= 0)
                throw new ValidacionException("Debes seleccionar una unidad.");
        }

        public void EliminarServicio(int id) => _servicioRepository.Eliminar(id);
    }
}
