using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class OperadoresController : Controller
    {
        private readonly OperadorService _operadorService;

        public OperadoresController(OperadorService operadorService)
        {
            _operadorService = operadorService;
        }

        public IActionResult Index()
        {
            return View(_operadorService.ObtenerTodos());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Operador operador)
        {
            if (!ModelState.IsValid)
                return View(operador);

            _operadorService.RegistrarOperador(operador);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _operadorService.EliminarOperador(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "No se puede eliminar: este operador tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
