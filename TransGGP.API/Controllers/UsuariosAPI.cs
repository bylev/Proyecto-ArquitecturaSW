using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Exceptions;
using TransGGP.Application.Security;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api")]
    public class UsuariosApiController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosApiController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("obtenerusuarios")]
        public ActionResult<List<UsuarioRespuestaDto>> ObtenerTodos()
        {
            return Ok(_usuarioService.ObtenerTodos().Select(Mapear).ToList());
        }

        [HttpPatch("cambiarrolusuario/{id}")]
        public IActionResult CambiarRol(int id, [FromBody] UsuarioCambiarRolDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Rol))
                return BadRequest("El rol es obligatorio.");

            var usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();

            if (EsUsuarioActual(usuario))
                return BadRequest("No puedes cambiar el rol de tu propia cuenta desde esta sesión.");

            try
            {
                var actualizado = _usuarioService.CambiarRol(id, dto.Rol);
                return Ok(Mapear(actualizado));
            }
            catch (ValidacionException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("actualizarpermisosusuario/{id}")]
        public IActionResult ActualizarPermisos(int id, [FromBody] PermisosUsuario? dto)
        {
            if (dto == null)
                return BadRequest("Los permisos son obligatorios.");

            var usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();

            if (EsUsuarioActual(usuario))
                return BadRequest("No puedes cambiar los permisos de tu propia cuenta desde esta sesión.");

            try
            {
                var actualizado = _usuarioService.ActualizarPermisos(id, dto);
                return Ok(Mapear(actualizado));
            }
            catch (ValidacionException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("eliminarusuario/{id}")]
        public IActionResult Eliminar(int id)
        {
            var usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();

            if (EsUsuarioActual(usuario))
                return BadRequest("No puedes eliminar tu propia cuenta desde esta sesión.");

            try
            {
                _usuarioService.EliminarUsuario(id);
                return NoContent();
            }
            catch (ValidacionException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool EsUsuarioActual(Usuario usuario)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var usuarioActualId) && usuarioActualId == usuario.Id)
                return true;

            var emailClaim = User.FindFirstValue(ClaimTypes.Email);
            return string.Equals(emailClaim, usuario.Email, StringComparison.OrdinalIgnoreCase);
        }

        private static UsuarioRespuestaDto Mapear(Usuario usuario)
        {
            return new UsuarioRespuestaDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol,
                PuedeVerServicios = usuario.PuedeVerServicios,
                PuedeEditarServicios = usuario.PuedeEditarServicios,
                PuedeVerClientes = usuario.PuedeVerClientes,
                PuedeEditarClientes = usuario.PuedeEditarClientes,
                PuedeVerOperadores = usuario.PuedeVerOperadores,
                PuedeEditarOperadores = usuario.PuedeEditarOperadores,
                PuedeVerUnidades = usuario.PuedeVerUnidades,
                PuedeEditarUnidades = usuario.PuedeEditarUnidades,
                PuedeVerSemirremolques = usuario.PuedeVerSemirremolques,
                PuedeEditarSemirremolques = usuario.PuedeEditarSemirremolques,
                PuedeVerDollys = usuario.PuedeVerDollys,
                PuedeEditarDollys = usuario.PuedeEditarDollys,
                PuedeVerConfiguraciones = usuario.PuedeVerConfiguraciones,
                PuedeEditarConfiguraciones = usuario.PuedeEditarConfiguraciones,
                FechaCreacion = usuario.FechaCreacion
            };
        }
    }

    public class UsuarioCambiarRolDto
    {
        public string Rol { get; set; } = string.Empty;
    }

    public class UsuarioRespuestaDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool PuedeVerServicios { get; set; }
        public bool PuedeEditarServicios { get; set; }
        public bool PuedeVerClientes { get; set; }
        public bool PuedeEditarClientes { get; set; }
        public bool PuedeVerOperadores { get; set; }
        public bool PuedeEditarOperadores { get; set; }
        public bool PuedeVerUnidades { get; set; }
        public bool PuedeEditarUnidades { get; set; }
        public bool PuedeVerSemirremolques { get; set; }
        public bool PuedeEditarSemirremolques { get; set; }
        public bool PuedeVerDollys { get; set; }
        public bool PuedeEditarDollys { get; set; }
        public bool PuedeVerConfiguraciones { get; set; }
        public bool PuedeEditarConfiguraciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
