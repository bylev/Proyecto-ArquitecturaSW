using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class SemirremolquesController : Controller
    {
        private readonly SemirremolqueService _semirremolqueService;

        public SemirremolquesController(SemirremolqueService semirremolqueService)
        {
            _semirremolqueService = semirremolqueService;
        }

        public IActionResult Index()
        {
            return View(_semirremolqueService.ObtenerTodos());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Semirremolque semirremolque)
        {
            if (!ModelState.IsValid)
                return View(semirremolque);

            _semirremolqueService.RegistrarSemirremolque(semirremolque);
            TempData["Exito"] = "Semirremolque guardado correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Semirremolque? semirremolque = _semirremolqueService.ObtenerPorId(id);
            if (semirremolque == null)
                return NotFound();
            return View(semirremolque);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Semirremolque semirremolque)
        {
            if (!ModelState.IsValid)
                return View(semirremolque);

            _semirremolqueService.ActualizarSemirremolque(semirremolque);
            TempData["Exito"] = "Semirremolque actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _semirremolqueService.EliminarSemirremolque(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "No se puede eliminar: este semirremolque tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
