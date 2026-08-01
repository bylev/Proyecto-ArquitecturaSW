using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Interfaces;
using TransGGP.Application.Security;
using TransGGP.Application.Services;

namespace TransGGP.Web.Controllers
{
    [Authorize]
    public class AsistenteController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly AsistenteService _asistenteService;

        public AsistenteController(DashboardService dashboardService, AsistenteService asistenteService)
        {
            _dashboardService = dashboardService;
            _asistenteService = asistenteService;
        }

        public class MensajeDto
        {
            public string Rol { get; set; } = "user";
            public string Contenido { get; set; } = string.Empty;
        }

        public class ChatPeticion
        {
            public int? Anio { get; set; }
            public List<MensajeDto> Mensajes { get; set; } = new List<MensajeDto>();
        }

        [HttpPost]
        public async Task<IActionResult> Preguntar([FromBody] ChatPeticion peticion)
        {
            if (!User.TienePermiso(Permisos.ServiciosLeer))
                return Forbid();

            if (peticion == null || peticion.Mensajes.Count == 0)
                return BadRequest(new { error = "No se recibió ningún mensaje." });

            var resumen = _dashboardService.Generar(peticion.Anio);

            var conversacion = peticion.Mensajes
                .Select(m => new MensajeChat { Rol = m.Rol, Contenido = m.Contenido })
                .ToList();

            try
            {
                var respuesta = await _asistenteService.ResponderAsync(resumen, conversacion);
                return Json(new { respuesta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
