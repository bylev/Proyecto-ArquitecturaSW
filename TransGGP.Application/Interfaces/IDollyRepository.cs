using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IDollyRepository
    {
        List<Dolly> ObtenerTodos();
        Dolly? ObtenerPorId(int id);
        void Agregar(Dolly dolly);
        void Actualizar(Dolly dolly);
        void Eliminar(int id);
    }
}
