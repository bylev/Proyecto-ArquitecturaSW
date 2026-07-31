using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Services;
using TransGGP.Domain.Models;

namespace TransGGP.Web.Controllers
{
    public class DollysController : Controller
    {
        private readonly DollyService _dollyService;

        public DollysController(DollyService dollyService)
        {
            _dollyService = dollyService;
        }

        public IActionResult Index()
        {
            return View(_dollyService.ObtenerTodos());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(Dolly dolly)
        {
            if (!ModelState.IsValid)
                return View(dolly);

            _dollyService.RegistrarDolly(dolly);
            TempData["Exito"] = "Dolly guardado correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Dolly? dolly = _dollyService.ObtenerPorId(id);
            if (dolly == null)
                return NotFound();
            return View(dolly);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Dolly dolly)
        {
            if (!ModelState.IsValid)
                return View(dolly);

            _dollyService.ActualizarDolly(dolly);
            TempData["Exito"] = "Dolly actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _dollyService.EliminarDolly(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "No se puede eliminar: este dolly tiene servicios registrados.";
            }
            return RedirectToAction("Index");
        }
    }
}
