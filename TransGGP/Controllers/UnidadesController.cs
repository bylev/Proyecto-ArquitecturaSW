using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Security;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    [Authorize(Policy = Permisos.UnidadesLeer)]
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

        [Authorize(Policy = Permisos.UnidadesEditar)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = Permisos.UnidadesEditar)]
        [HttpPost]
        public IActionResult Create(Unidad unidad)
        {
            if (!ModelState.IsValid)
                return View(unidad);

            _unidadService.RegistrarUnidad(unidad);
            TempData["Exito"] = "Unidad guardada correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permisos.UnidadesEditar)]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Unidad? unidad = _unidadService.ObtenerPorId(id);
            if (unidad == null)
                return NotFound();
            return View(unidad);
        }

        [Authorize(Policy = Permisos.UnidadesEditar)]
        [HttpPost]
        public IActionResult Edit(Unidad unidad)
        {
            if (!ModelState.IsValid)
                return View(unidad);

            _unidadService.ActualizarUnidad(unidad);
            TempData["Exito"] = "Unidad actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permisos.UnidadesEditar)]
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
