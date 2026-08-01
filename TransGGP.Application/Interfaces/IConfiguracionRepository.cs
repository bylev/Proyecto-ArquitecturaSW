using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IConfiguracionRepository
    {
        List<Configuracion> ObtenerTodos();
        Configuracion? ObtenerPorId(int id);
        void Agregar(Configuracion configuracion);
        void Actualizar(Configuracion configuracion);
        void Eliminar(int id);
    }
}
