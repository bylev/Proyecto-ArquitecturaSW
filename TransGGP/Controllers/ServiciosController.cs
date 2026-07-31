using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index(string? buscar, int pagina = 1)
        {
            var servicios = _servicioService.ObtenerTodos();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim().ToLower();
                servicios = servicios
                    .Where(s => s.NumeroEmbarque.ToLower().Contains(texto)
                             || s.Origen.ToLower().Contains(texto)
                             || s.Destino.ToLower().Contains(texto)
                             || s.Estatus.ToLower().Contains(texto))
                    .ToList();
            }

            int porPagina = 10;
            int totalPaginas = (int)Math.Ceiling(servicios.Count / (double)porPagina);
            if (pagina < 1) pagina = 1;
            if (totalPaginas > 0 && pagina > totalPaginas) pagina = totalPaginas;

            var serviciosPagina = servicios
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToList();

            ViewBag.Buscar = buscar;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(serviciosPagina);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            CargarDropdowns();
            return View(new Servicio()); // fechas con valor por defecto (DateTime.Now)
        }

        [Authorize(Roles = "Admin")]
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
            TempData["Exito"] = "Servicio guardado correctamente.";
            return RedirectToAction("Index");
        }

        // GET Edit: muestra el formulario con los datos del servicio
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var servicio = _servicioService.ObtenerPorId(id);
            if (servicio == null)
                return NotFound();

            CargarDropdowns();
            return View(servicio);
        }

        // POST Edit: recibe los cambios y guarda
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                CargarDropdowns();
                return View(servicio);
            }

            // Red de seguridad: evita fechas inválidas para MySQL
            if (servicio.FechaCarga < new DateTime(2000, 1, 1)) servicio.FechaCarga = DateTime.Now;
            if (servicio.FechaEntrega < new DateTime(2000, 1, 1)) servicio.FechaEntrega = DateTime.Now;

            _servicioService.ActualizarServicio(servicio);
            TempData["Exito"] = "Servicio actualizado correctamente.";
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
