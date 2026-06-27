using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ServicioService _servicioService;
        private readonly ClienteService _clienteService;
        private readonly OperadorService _operadorService;
        private readonly UnidadService _unidadService;

        // Se inyectan 4 servicios: el de Servicio + los 3 que llenan los dropdowns
        public ServiciosController(
            ServicioService servicioService,
            ClienteService clienteService,
            OperadorService operadorService,
            UnidadService unidadService)
        {
            _servicioService = servicioService;
            _clienteService = clienteService;
            _operadorService = operadorService;
            _unidadService = unidadService;
        }

        public IActionResult Index()
        {
            return View(_servicioService.ObtenerTodos());
        }

        [HttpGet]
        public IActionResult Create()
        {
            CargarDropdowns();
            return View(new Servicio()); // fechas con valor por defecto (DateTime.Now)
        }

        [HttpPost]
        public IActionResult Create(Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                CargarDropdowns();
                return View(servicio);
            }

            // Red de seguridad: evita fechas inválidas para MySQL
            if (servicio.FechaCarga < new DateTime(2000, 1, 1)) servicio.FechaCarga = DateTime.Now;
            if (servicio.FechaEntrega < new DateTime(2000, 1, 1)) servicio.FechaEntrega = DateTime.Now;
            servicio.FechaCreacion = DateTime.Now;

            _servicioService.RegistrarServicio(servicio);
            return RedirectToAction("Index");
        }

        // Nota: los servicios NO se eliminan. Son historial para analizar
        // el crecimiento de la empresa (registro permanente / append-only).

        // Llena los menús desplegables con los datos reales de cada catálogo
        private void CargarDropdowns()
        {
            ViewBag.Clientes = new SelectList(_clienteService.ObtenerTodos(), "Id", "Nombre");
            ViewBag.Operadores = new SelectList(_operadorService.ObtenerTodos(), "Id", "Nombre");
            ViewBag.Unidades = new SelectList(_unidadService.ObtenerTodos(), "Id", "Placa");
        }
    }
}
