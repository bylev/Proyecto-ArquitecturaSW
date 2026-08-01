using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

namespace TransGGP.Application.Services
{
    public class ConfiguracionService
    {
        private readonly IConfiguracionRepository _configuracionRepository;

        public ConfiguracionService(IConfiguracionRepository configuracionRepository)
        {
            _configuracionRepository = configuracionRepository;
        }

        public List<Configuracion> ObtenerTodos() => _configuracionRepository.ObtenerTodos();

        public Configuracion? ObtenerPorId(int id) => _configuracionRepository.ObtenerPorId(id);

        public Configuracion RegistrarConfiguracion(Configuracion configuracion)
        {
            Validar(configuracion);
            _configuracionRepository.Agregar(configuracion);
            return configuracion;
        }

        public void ActualizarConfiguracion(Configuracion configuracion)
        {
            Validar(configuracion);
            _configuracionRepository.Actualizar(configuracion);
        }

        public void EliminarConfiguracion(int id) => _configuracionRepository.Eliminar(id);

        private void Validar(Configuracion configuracion)
        {
            if (string.IsNullOrWhiteSpace(configuracion.Nombre))
                throw new ValidacionException("El nombre de la configuración es obligatorio.");
        }
    }
}
