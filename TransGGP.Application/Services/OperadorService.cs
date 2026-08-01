using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;

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
            Validar(operador);
            _operadorRepository.Agregar(operador);
            return operador;
        }

        public void ActualizarOperador(Operador operador)
        {
            Validar(operador);
            _operadorRepository.Actualizar(operador);
        }

        public void EliminarOperador(int id) => _operadorRepository.Eliminar(id);

        private void Validar(Operador operador)
        {
            if (string.IsNullOrWhiteSpace(operador.NumeroOperador))
                throw new ValidacionException("El número de operador es obligatorio.");
            if (string.IsNullOrWhiteSpace(operador.Nombre))
                throw new ValidacionException("El nombre del operador es obligatorio.");
        }
    }
}
