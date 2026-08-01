using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TransGGP.Application.Security;
using TransGGP.Application.Services;

namespace TransGGP.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthApiController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly IConfiguration _config;

        public AuthApiController(UsuarioService usuarioService, IConfiguration config)
        {
            _usuarioService = usuarioService;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Correo y contraseña son obligatorios.");

            var usuario = _usuarioService.ValidarCredenciales(dto.Email, dto.Password);
            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };
            claims.AddRange(Permisos.ObtenerPermisos(usuario)
                .Select(permiso => new Claim(Permisos.ClaimType, permiso)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expira = token.ValidTo,
                rol = usuario.Rol,
                permisos = Permisos.ObtenerPermisos(usuario)
            });
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
