using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IServicioRepository
    {
        List<Servicio> ObtenerTodos();
        Servicio? ObtenerPorId(int id);
        void Agregar(Servicio servicio);
        void Actualizar(Servicio servicio);
        void Eliminar(int id);
    }
}
