using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Infrastructure.Data;

namespace TransGGP.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Usuario> ObtenerTodos()
        {
            return _context.Usuarios.ToList();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _context.Usuarios.Find(id);
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email);
        }

        public void Agregar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Actualizar(Usuario usuario)
        {
            var existente = _context.Usuarios.Find(usuario.Id);
            if (existente != null)
            {
                existente.NombreCompleto = usuario.NombreCompleto;
                existente.Email = usuario.Email;
                existente.Password = usuario.Password;
                existente.Rol = usuario.Rol;
                existente.PuedeVerServicios = usuario.PuedeVerServicios;
                existente.PuedeEditarServicios = usuario.PuedeEditarServicios;
                existente.PuedeVerClientes = usuario.PuedeVerClientes;
                existente.PuedeEditarClientes = usuario.PuedeEditarClientes;
                existente.PuedeVerOperadores = usuario.PuedeVerOperadores;
                existente.PuedeEditarOperadores = usuario.PuedeEditarOperadores;
                existente.PuedeVerUnidades = usuario.PuedeVerUnidades;
                existente.PuedeEditarUnidades = usuario.PuedeEditarUnidades;
                existente.PuedeVerSemirremolques = usuario.PuedeVerSemirremolques;
                existente.PuedeEditarSemirremolques = usuario.PuedeEditarSemirremolques;
                existente.PuedeVerDollys = usuario.PuedeVerDollys;
                existente.PuedeEditarDollys = usuario.PuedeEditarDollys;
                existente.PuedeVerConfiguraciones = usuario.PuedeVerConfiguraciones;
                existente.PuedeEditarConfiguraciones = usuario.PuedeEditarConfiguraciones;
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
        }
    }
}
