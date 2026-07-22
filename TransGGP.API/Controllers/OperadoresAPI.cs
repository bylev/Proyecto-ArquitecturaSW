using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class OperadoresApiController : ControllerBase
    {
        private readonly OperadorService _operadorService;

        public OperadoresApiController(OperadorService operadorService)
        {
            _operadorService = operadorService;
        }

        [HttpGet("obteneroperadores")]
        public ActionResult<List<Operador>> ObtenerTodos()
        {
            return Ok(_operadorService.ObtenerTodos());
        }

        [HttpGet("obteneroperador/{id}")]
        public ActionResult<Operador> ObtenerPorId(int id)
        {
            var operador = _operadorService.ObtenerPorId(id);
            if (operador == null) return NotFound();
            return Ok(operador);
        }

        [HttpPost("crearoperador")]
        public ActionResult<Operador> Crear([FromBody] Operador operador)
        {
            if (string.IsNullOrWhiteSpace(operador.Nombre))
                return BadRequest("El nombre del operador es requerido");

            var creado = _operadorService.RegistrarOperador(operador);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpDelete("eliminaroperador/{id}")]
        public IActionResult Eliminar(int id)
        {
            _operadorService.EliminarOperador(id);
            return NoContent();
        }
    }
}
