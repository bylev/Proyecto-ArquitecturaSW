using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IUnidadRepository
    {
        List<Unidad> ObtenerTodos();
        Unidad? ObtenerPorId(int id);
        void Agregar(Unidad unidad);
        void Actualizar(Unidad unidad);
        void Eliminar(int id);
    }
}
