using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class ConfiguracionesController : Controller
    {
        private readonly ConfiguracionService _configuracionService;

        public ConfiguracionesController(ConfiguracionService configuracionService)
        {
            _configuracionService = configuracionService;
        }

        public IActionResult Index()
        {
            return View(_configuracionService.ObtenerTodos());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Configuracion configuracion)
        {
            if (!ModelState.IsValid)
                return View(configuracion);

            _configuracionService.RegistrarConfiguracion(configuracion);
            TempData["Exito"] = "Configuración guardada correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Configuracion? configuracion = _configuracionService.ObtenerPorId(id);
            if (configuracion == null)
                return NotFound();
            return View(configuracion);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Configuracion configuracion)
        {
            if (!ModelState.IsValid)
                return View(configuracion);

            _configuracionService.ActualizarConfiguracion(configuracion);
            TempData["Exito"] = "Configuración actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _configuracionService.EliminarConfiguracion(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "No se puede eliminar: esta configuración tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
