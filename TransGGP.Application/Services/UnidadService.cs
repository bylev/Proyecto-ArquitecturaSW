using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;

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
            _unidadRepository.Agregar(unidad);
            return unidad;
        }

        public void ActualizarUnidad(Unidad unidad)
        {
            _unidadRepository.Actualizar(unidad);
        }

        public void EliminarUnidad(int id) => _unidadRepository.Eliminar(id);
    }
}
