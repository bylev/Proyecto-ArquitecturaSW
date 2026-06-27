using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class UnidadesController : Controller
    {
        private readonly UnidadService _unidadService;

        public UnidadesController(UnidadService unidadService)
        {
            _unidadService = unidadService;
        }

        public IActionResult Index()
        {
            return View(_unidadService.ObtenerTodos());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Unidad unidad)
        {
            if (!ModelState.IsValid)
                return View(unidad);

            _unidadService.RegistrarUnidad(unidad);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _unidadService.EliminarUnidad(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "No se puede eliminar: esta unidad tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
