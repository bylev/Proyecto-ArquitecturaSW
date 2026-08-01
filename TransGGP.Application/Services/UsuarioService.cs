using TransGGP.Domain.Models;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Exceptions;
using TransGGP.Application.Security;

namespace TransGGP.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public List<Usuario> ObtenerTodos() => _usuarioRepository.ObtenerTodos();

        public Usuario? ObtenerPorId(int id) => _usuarioRepository.ObtenerPorId(id);

        public bool ExisteAlgunUsuario() => _usuarioRepository.ObtenerTodos().Count > 0;

        public bool ExisteEmail(string email) => _usuarioRepository.ObtenerPorEmail((email ?? string.Empty).Trim()) != null;

        public Usuario RegistrarUsuario(string nombreCompleto, string email, string password, string rol, PermisosUsuario? permisos = null)
        {
            var emailNormalizado = (email ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nombreCompleto))
                throw new ValidacionException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(emailNormalizado))
                throw new ValidacionException("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ValidacionException("La contraseña debe tener al menos 6 caracteres.");

            var rolNormalizado = NormalizarRol(rol);

            if (ExisteEmail(emailNormalizado))
                throw new ValidacionException("Ya existe un usuario con ese correo.");

            var usuario = new Usuario
            {
                NombreCompleto = nombreCompleto.Trim(),
                Email = emailNormalizado,
                Password = _passwordHasher.Hashear(password),
                Rol = rolNormalizado
            };

            (rolNormalizado == "Admin" ? PermisosUsuario.Todos() : permisos ?? new PermisosUsuario())
                .AplicarA(usuario);

            _usuarioRepository.Agregar(usuario);
            return usuario;
        }

        public Usuario CambiarRol(int id, string rol)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id)
                ?? throw new ValidacionException("No se encontró el usuario.");

            var rolNormalizado = NormalizarRol(rol);

            if (usuario.Rol == "Admin" && rolNormalizado != "Admin" && EsUltimoAdministrador(usuario.Id))
                throw new ValidacionException("Debe existir al menos un usuario administrador.");

            usuario.Rol = rolNormalizado;
            if (rolNormalizado == "Admin")
                PermisosUsuario.Todos().AplicarA(usuario);

            _usuarioRepository.Actualizar(usuario);

            return usuario;
        }

        public Usuario ActualizarPermisos(int id, PermisosUsuario permisos)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id)
                ?? throw new ValidacionException("No se encontró el usuario.");

            if (usuario.Rol == "Admin")
                PermisosUsuario.Todos().AplicarA(usuario);
            else
                permisos.AplicarA(usuario);

            _usuarioRepository.Actualizar(usuario);

            return usuario;
        }

        public void EliminarUsuario(int id)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id)
                ?? throw new ValidacionException("No se encontró el usuario.");

            if (usuario.Rol == "Admin" && EsUltimoAdministrador(usuario.Id))
                throw new ValidacionException("No se puede eliminar el único usuario administrador.");

            _usuarioRepository.Eliminar(id);
        }

        public void CambiarPassword(int id, string passwordActual, string passwordNueva)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id)
                ?? throw new ValidacionException("No se encontró el usuario.");

            if (!_passwordHasher.Verificar(passwordActual, usuario.Password))
                throw new ValidacionException("La contraseña actual no es correcta.");

            if (string.IsNullOrWhiteSpace(passwordNueva) || passwordNueva.Length < 6)
                throw new ValidacionException("La nueva contraseña debe tener al menos 6 caracteres.");

            usuario.Password = _passwordHasher.Hashear(passwordNueva);
            _usuarioRepository.Actualizar(usuario);
        }

        public void ResetearPassword(int id, string passwordNueva)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id)
                ?? throw new ValidacionException("No se encontró el usuario.");

            if (string.IsNullOrWhiteSpace(passwordNueva) || passwordNueva.Length < 6)
                throw new ValidacionException("La nueva contraseña debe tener al menos 6 caracteres.");

            usuario.Password = _passwordHasher.Hashear(passwordNueva);
            _usuarioRepository.Actualizar(usuario);
        }

        public Usuario? ValidarCredenciales(string email, string password)
        {
            var usuario = _usuarioRepository.ObtenerPorEmail((email ?? string.Empty).Trim());
            if (usuario == null)
                return null;

            if (!_passwordHasher.Verificar(password, usuario.Password))
                return null;

            return usuario;
        }

        private bool EsUltimoAdministrador(int usuarioId)
        {
            return !_usuarioRepository.ObtenerTodos()
                .Any(u => u.Id != usuarioId && u.Rol == "Admin");
        }

        private static string NormalizarRol(string rol)
        {
            if (string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
                return "Admin";

            if (string.Equals(rol, "User", StringComparison.OrdinalIgnoreCase))
                return "User";

            throw new ValidacionException("El rol debe ser Admin o User.");
        }
    }
}
