using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UnidadesApiController : ControllerBase
    {
        private readonly UnidadService _unidadService;

        public UnidadesApiController(UnidadService unidadService)
        {
            _unidadService = unidadService;
        }

        [HttpGet("obtenerunidades")]
        public ActionResult<List<Unidad>> ObtenerTodos()
        {
            return Ok(_unidadService.ObtenerTodos());
        }

        [HttpGet("obtenerunidad/{id}")]
        public ActionResult<Unidad> ObtenerPorId(int id)
        {
            var unidad = _unidadService.ObtenerPorId(id);
            if (unidad == null) return NotFound();
            return Ok(unidad);
        }

        [HttpPost("crearunidad")]
        public ActionResult<Unidad> Crear([FromBody] Unidad unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad.Placa))
                return BadRequest("La placa de la unidad es requerida");

            var creada = _unidadService.RegistrarUnidad(unidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }

        [HttpDelete("eliminarunidad/{id}")]
        public IActionResult Eliminar(int id)
        {
            _unidadService.EliminarUnidad(id);
            return NoContent();
        }
    }
}
