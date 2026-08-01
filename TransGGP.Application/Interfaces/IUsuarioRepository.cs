using TransGGP.Domain.Models;

namespace TransGGP.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        void Agregar(Usuario usuario);
        void Actualizar(Usuario usuario);
        void Eliminar(int id);
    }
}
