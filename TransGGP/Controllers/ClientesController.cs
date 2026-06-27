using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Application.Reports;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class ClientesController : Controller // Hereda de Controller
    {
        private readonly ClienteService _clienteService;

        // Constructor
        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService; // Inyección de dependencias del servicio
        }
      
        // Index
        public IActionResult Index()
        {
            List<Cliente> clientes = _clienteService.ObtenerTodos(); // Llama al servicio para obtener todos los clientes
            return View(clientes);
        }

        // GET Create: muestra el formulario vacío
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST Create: recibe los datos del formulario y guarda
        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente); // si los datos no son válidos, vuelve a mostrar el formulario

            _clienteService.RegistrarCliente(cliente); // guarda el cliente
            return RedirectToAction("Index"); // tras guardar, redirige a la lista
        }

        // GET Reporte: usa el PATRÓN FACTORY METHOD para generar el reporte
        // en el formato pedido (texto o csv). El controlador NO sabe cómo se
        // arma cada formato: solo elige el "creador" y le pide el resultado.
        [HttpGet]
        public IActionResult Reporte(string formato = "texto")
        {
            List<Cliente> clientes = _clienteService.ObtenerTodos();

            ReporteCreator creator = formato == "csv"
                ? new ReporteCsvCreator()
                : new ReporteTextoCreator();

            string contenido = creator.GenerarReporte(clientes);
            return Content(contenido, "text/plain"); // muestra el texto en el navegador
        }

        // Acciones que reciben un id : Edit y Delete
        // GET Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Cliente? cliente = _clienteService.ObtenerPorId(id); // Llama al servicio para obtener un cliente por id
            if (cliente == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el cliente
            }
            return View(cliente); // Retorna la vista con el cliente encontrado
        }

        // POST Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _clienteService.EliminarCliente(id);
            }
            catch (Exception)
            {
                // La base restringe el borrado si el cliente tiene servicios (historial)
                TempData["Error"] = "No se puede eliminar: este cliente tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
