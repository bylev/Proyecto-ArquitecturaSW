using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

namespace TransGGP.Application.Services
{
    public class UnidadService
    {
        private readonly IUnidadRepository _unidadRepository;

        public UnidadService(IUnidadRepository unidadRepository)
        {
            _unidadRepository = unidadRepository;
        }

        public List<Unidad> ObtenerTodos() => _unidadRepository.ObtenerTodos();

        public Unidad? ObtenerPorId(int id) => _unidadRepository.ObtenerPorId(id);

        public Unidad RegistrarUnidad(Unidad unidad)
        {
            Validar(unidad);
            _unidadRepository.Agregar(unidad);
            return unidad;
        }

        public void ActualizarUnidad(Unidad unidad)
        {
            Validar(unidad);
            _unidadRepository.Actualizar(unidad);
        }

        public void EliminarUnidad(int id) => _unidadRepository.Eliminar(id);

        private void Validar(Unidad unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad.Clave))
                throw new ValidacionException("La clave de la unidad es obligatoria.");
            if (string.IsNullOrWhiteSpace(unidad.Placa))
                throw new ValidacionException("La placa de la unidad es obligatoria.");
        }
    }
}
