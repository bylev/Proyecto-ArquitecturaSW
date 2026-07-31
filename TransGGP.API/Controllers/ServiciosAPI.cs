using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Security;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [Authorize(Policy = Permisos.ServiciosLeer)]
    [ApiController]
    [Route("api")]
    public class ServiciosApiController : ControllerBase
    {
        private readonly ServicioService _servicioService;

        public ServiciosApiController(ServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet("obtenerservicios")]
        public ActionResult<List<Servicio>> ObtenerTodos()
        {
            return Ok(_servicioService.ObtenerTodos());
        }

        [HttpGet("obtenerservicio/{id}")]
        public ActionResult<Servicio> ObtenerPorId(int id)
        {
            var servicio = _servicioService.ObtenerPorId(id);
            if (servicio == null) return NotFound();
            return Ok(servicio);
        }

        [Authorize(Policy = Permisos.ServiciosEditar)]
        [HttpPost("crearservicio")]
        public ActionResult<Servicio> Crear([FromBody] Servicio servicio)
        {
            if (servicio.ClienteId <= 0 || servicio.OperadorId <= 0 || servicio.UnidadId <= 0)
                return BadRequest("Cliente, Operador y Unidad son requeridos");

            // Red de seguridad para fechas válidas en MySQL
            if (servicio.FechaCarga < new DateTime(2000, 1, 1)) servicio.FechaCarga = DateTime.Now;
            if (servicio.FechaEntrega < new DateTime(2000, 1, 1)) servicio.FechaEntrega = DateTime.Now;
            servicio.FechaCreacion = DateTime.Now;

            var creado = _servicioService.RegistrarServicio(servicio);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        // Nota: no se expone DELETE. Los servicios son historial permanente
        // para análisis de crecimiento, no se eliminan.
    }
}
