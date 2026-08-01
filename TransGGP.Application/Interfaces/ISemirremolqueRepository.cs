using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface ISemirremolqueRepository
    {
        List<Semirremolque> ObtenerTodos();
        Semirremolque? ObtenerPorId(int id);
        void Agregar(Semirremolque semirremolque);
        void Actualizar(Semirremolque semirremolque);
        void Eliminar(int id);
    }
}
