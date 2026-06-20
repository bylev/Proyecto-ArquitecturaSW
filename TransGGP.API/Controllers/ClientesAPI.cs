using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.DTOs;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesApiController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesApiController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        
        [HttpGet]
        public ActionResult<List<Cliente>> ObtenerTodos()
        {
            var clientes = _clienteService.ObtenerTodos();
            return Ok(clientes);
        }

      
        [HttpPost]
 
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
    }
}