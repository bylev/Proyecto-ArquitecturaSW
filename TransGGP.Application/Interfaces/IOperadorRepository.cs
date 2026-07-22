using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IOperadorRepository
    {
        List<Operador> ObtenerTodos();
        Operador? ObtenerPorId(int id);
        void Agregar(Operador operador);
        void Eliminar(int id);
    }
}
