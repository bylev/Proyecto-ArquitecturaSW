using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransGGP.Application.Exceptions;
using TransGGP.Application.Services;
using TransGGP.ViewModels;

namespace TransGGP.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View(_usuarioService.ObtenerTodos());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UsuarioCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            try
            {
                _usuarioService.RegistrarUsuario(modelo.NombreCompleto, modelo.Email, modelo.Password, modelo.Rol);
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }

            TempData["Exito"] = "Usuario creado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarRol(int id, string rol)
        {
            if (EsUsuarioActual(id))
            {
                TempData["Error"] = "No puedes cambiar el rol de tu propia cuenta desde esta sesión.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _usuarioService.CambiarRol(id, rol);
                TempData["Exito"] = "Rol actualizado correctamente.";
            }
            catch (ValidacionException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (EsUsuarioActual(id))
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta desde esta sesión.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _usuarioService.EliminarUsuario(id);
                TempData["Exito"] = "Usuario eliminado correctamente.";
            }
            catch (ValidacionException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EsUsuarioActual(int id)
        {
            var idUsuarioActual = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idUsuarioActual, out var usuarioActualId) && usuarioActualId == id)
                return true;

            var usuario = _usuarioService.ObtenerPorId(id);
            var emailUsuarioActual = User.FindFirstValue(ClaimTypes.Email);

            return usuario != null
                && string.Equals(emailUsuarioActual, usuario.Email, StringComparison.OrdinalIgnoreCase);
        }
    }
}
