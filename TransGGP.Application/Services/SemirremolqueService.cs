using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

namespace TransGGP.Application.Services
{
    public class SemirremolqueService
    {
        private readonly ISemirremolqueRepository _semirremolqueRepository;

        public SemirremolqueService(ISemirremolqueRepository semirremolqueRepository)
        {
            _semirremolqueRepository = semirremolqueRepository;
        }

        public List<Semirremolque> ObtenerTodos() => _semirremolqueRepository.ObtenerTodos();

        public Semirremolque? ObtenerPorId(int id) => _semirremolqueRepository.ObtenerPorId(id);

        public Semirremolque RegistrarSemirremolque(Semirremolque semirremolque)
        {
            Validar(semirremolque);
            _semirremolqueRepository.Agregar(semirremolque);
            return semirremolque;
        }

        public void ActualizarSemirremolque(Semirremolque semirremolque)
        {
            Validar(semirremolque);
            _semirremolqueRepository.Actualizar(semirremolque);
        }

        public void EliminarSemirremolque(int id) => _semirremolqueRepository.Eliminar(id);

        private void Validar(Semirremolque semirremolque)
        {
            if (string.IsNullOrWhiteSpace(semirremolque.Clave))
                throw new ValidacionException("La clave del semirremolque es obligatoria.");
            if (string.IsNullOrWhiteSpace(semirremolque.Placa))
                throw new ValidacionException("La placa del semirremolque es obligatoria.");
            if (string.IsNullOrWhiteSpace(semirremolque.Tipo))
                throw new ValidacionException("El tipo del semirremolque es obligatorio.");
        }
    }
}
