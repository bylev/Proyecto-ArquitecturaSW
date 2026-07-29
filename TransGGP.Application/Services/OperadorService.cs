using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;

namespace TransGGP.Application.Services
{
    public class OperadorService
    {
        private readonly IOperadorRepository _operadorRepository;

        public OperadorService(IOperadorRepository operadorRepository)
        {
            _operadorRepository = operadorRepository;
        }

        public List<Operador> ObtenerTodos() => _operadorRepository.ObtenerTodos();

        public Operador? ObtenerPorId(int id) => _operadorRepository.ObtenerPorId(id);

        public Operador RegistrarOperador(Operador operador)
        {
            _operadorRepository.Agregar(operador);
            return operador;
        }

        public void ActualizarOperador(Operador operador)
        {
            _operadorRepository.Actualizar(operador);
        }

        public void EliminarOperador(int id) => _operadorRepository.Eliminar(id);
    }
}
