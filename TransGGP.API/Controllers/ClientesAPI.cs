using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.DTOs;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ClientesApiController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesApiController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        
        [HttpGet("obtenerclientes")]
        public ActionResult<List<Cliente>> ObtenerTodos()
        {
            var clientes = _clienteService.ObtenerTodos();
            return Ok(clientes);
        }

      
        [HttpPost("crearcliente")]
        public ActionResult<Cliente> CrearCliente([FromBody] ClienteCreateDto clienteDto)
        {
            if (string.IsNullOrWhiteSpace(clienteDto.Nombre))
                return BadRequest("El nombre del cliente es requerido");

            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                FechaCreacion = DateTime.Now
            };

            var clienteCreado = _clienteService.RegistrarCliente(cliente);
            return CreatedAtAction(nameof(ObtenerTodos), new { id = clienteCreado.Id }, clienteCreado);
        }

        [HttpDelete("eliminarcliente/{id}")]
        public IActionResult Eliminar(int id)
        {
            _clienteService.EliminarCliente(id);
            return NoContent();
        }
    }
}