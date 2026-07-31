using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

namespace TransGGP.Application.Services
{
    public class DollyService
    {
        private readonly IDollyRepository _dollyRepository;

        public DollyService(IDollyRepository dollyRepository)
        {
            _dollyRepository = dollyRepository;
        }

        public List<Dolly> ObtenerTodos() => _dollyRepository.ObtenerTodos();

        public Dolly? ObtenerPorId(int id) => _dollyRepository.ObtenerPorId(id);

        public Dolly RegistrarDolly(Dolly dolly)
        {
            Validar(dolly);
            _dollyRepository.Agregar(dolly);
            return dolly;
        }

        public void ActualizarDolly(Dolly dolly)
        {
            Validar(dolly);
            _dollyRepository.Actualizar(dolly);
        }

        public void EliminarDolly(int id) => _dollyRepository.Eliminar(id);

        private void Validar(Dolly dolly)
        {
            if (string.IsNullOrWhiteSpace(dolly.Clave))
                throw new ValidacionException("La clave del dolly es obligatoria.");
            if (string.IsNullOrWhiteSpace(dolly.Placa))
                throw new ValidacionException("La placa del dolly es obligatoria.");
        }
    }
}
